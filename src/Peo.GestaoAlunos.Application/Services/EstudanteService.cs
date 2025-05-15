using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.Core.Interfaces.Services.Acls;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Application.Services;

public class EstudanteService(
    IEstudanteRepository estudanteRepository,
    ICursoAulaService cursoAulaService,
    IDetalhesUsuarioService detalhesUsuarioService,
    IAppIdentityUser appIdentityUser) : IEstudanteService
{
    public async Task<Estudante> CriarEstudanteAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var estudante = new Estudante(usuarioId);
        await estudanteRepository.AddAsync(estudante);
        await estudanteRepository.UnitOfWork.CommitAsync(cancellationToken);
        return estudante;
    }

    public async Task<Matricula> MatricularEstudanteAsync(Guid estudanteId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        var estudante = await estudanteRepository.GetByIdAsync(estudanteId);
        if (estudante == null)
            throw new ArgumentException("Estudante não encontrado", nameof(estudanteId));

        var cursoExiste = await cursoAulaService.ValidarSeCursoExisteAsync(cursoId);

        if (!cursoExiste)
            throw new ArgumentException("Curso não encontrado", nameof(cursoId));

        var cursoJaMatriculado = await estudanteRepository.AnyAsync(s => s.Id == estudanteId && s.Matriculas.Any(e => e.CursoId == cursoId));

        if (cursoJaMatriculado)
        {
            throw new ArgumentException("Estudante já está matriculado neste curso", nameof(cursoId));
        }

        var matricula = new Matricula(estudanteId, cursoId);
        await estudanteRepository.AddMatriculaAsync(matricula);
        await estudanteRepository.UnitOfWork.CommitAsync(cancellationToken);
        return matricula;
    }

    public async Task<Matricula> MatricularEstudanteComUserIdAsync(Guid usuarioId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        Estudante estudante = await ObterEstudantePorUserIdAsync(usuarioId, cancellationToken);

        return await MatricularEstudanteAsync(estudante.Id, cursoId, cancellationToken);
    }

    public async Task<Estudante> ObterEstudantePorUserIdAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var estudante = await estudanteRepository.GetByUserIdAsync(usuarioId);

        estudante ??= await CriarEstudanteAsync(usuarioId, cancellationToken);
        return estudante;
    }

    public async Task<ProgressoMatricula> IniciarAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default)
    {
        var matricula = await estudanteRepository.GetMatriculaByIdAsync(matriculaId)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarEstudanteEhUsuarioLogado(matricula, cancellationToken);

        if (matricula.Status != StatusMatricula.Ativo)
            throw new InvalidOperationException("Não é possível iniciar aula para matrícula inativa");

        var progressoExistente = await estudanteRepository.GetProgressoMatriculaAsync(matriculaId, aulaId);
        if (progressoExistente != null)
            throw new InvalidOperationException("Aula já iniciada");

        var progresso = new ProgressoMatricula(matriculaId, aulaId);
        await estudanteRepository.AddProgressoMatriculaAsync(progresso);
        await estudanteRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progresso;
    }

    private async Task ValidarEstudanteEhUsuarioLogado(Matricula matricula, CancellationToken cancellationToken)
    {
        var estudante = await ObterEstudantePorUserIdAsync(appIdentityUser.GetUserId(), cancellationToken);

        if (estudante.Id != matricula.EstudanteId)
        {
            throw new DomainException("Violação de isolamento detectada (usuário atual não é o estudante da matrícula)");
        }
    }

    public async Task<ProgressoMatricula> ConcluirAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default)
    {
        var matricula = await estudanteRepository.GetMatriculaByIdAsync(matriculaId)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarEstudanteEhUsuarioLogado(matricula, cancellationToken);

        var progresso = await estudanteRepository.GetProgressoMatriculaAsync(matriculaId, aulaId)
            ?? throw new ArgumentException("Aula não iniciada", nameof(aulaId));

        if (progresso.EstaConcluido)
            throw new InvalidOperationException("Aula já concluída");

        // Marcar aula como concluída
        progresso.MarcarComoConcluido();
        await estudanteRepository.AtualizarProgressoMatriculaAsync(progresso);

        // Calcular e atualizar progresso geral
        var totalAulas = await cursoAulaService.ObterTotalAulasDoCursoAsync(matricula.CursoId);
        var aulasConcluidas = await estudanteRepository.GetAulasConcluidasCountAsync(matriculaId);

        var novoPercentualProgresso = (int)(aulasConcluidas * 100.0 / totalAulas);
        matricula.AtualizarProgresso(novoPercentualProgresso);

        // Se todas as aulas foram concluídas, marcar matrícula como concluída
        if (novoPercentualProgresso == 100)
        {
            matricula.Concluir();
        }

        await estudanteRepository.AtualizarMatricula(matricula);
        await estudanteRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progresso;
    }

    public async Task<int> ObterProgressoGeralCursoAsync(Guid matriculaId, CancellationToken cancellationToken = default)
    {
        var matricula = await estudanteRepository.GetMatriculaByIdAsync(matriculaId)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarEstudanteEhUsuarioLogado(matricula, cancellationToken);

        return matricula.PercentualProgresso;
    }

    public async Task<Matricula> ConcluirMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken = default)
    {
        var matricula = await estudanteRepository.GetMatriculaByIdAsync(matriculaId)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarEstudanteEhUsuarioLogado(matricula, cancellationToken);

        if (matricula.Status != StatusMatricula.Ativo)
            throw new InvalidOperationException($"Não é possível concluir matrícula no status {matricula.Status}");

        // Verificar se todas as aulas foram concluídas
        var totalAulas = await cursoAulaService.ObterTotalAulasDoCursoAsync(matricula.CursoId);
        var aulasConcluidas = await estudanteRepository.GetAulasConcluidasCountAsync(matriculaId);

        if (aulasConcluidas < totalAulas)
            throw new InvalidOperationException($"Não é possível concluir matrícula. {aulasConcluidas} de {totalAulas} aulas concluídas.");

        matricula.Concluir();
        await estudanteRepository.AtualizarMatricula(matricula);

        var numeroCertificado = GerarNumeroCertificado();

        var certificado = new Certificado(
            matriculaId: matricula.Id,
            conteudo: await GerarConteudoCertificadoAsync(matricula, numeroCertificado),
            dataEmissao: DateTime.Now,
            numeroCertificado: numeroCertificado
        );
        await estudanteRepository.AddCertificadoAsync(certificado);

        await estudanteRepository.UnitOfWork.CommitAsync(cancellationToken);

        return matricula;
    }

    private async Task<string> GerarConteudoCertificadoAsync(Matricula matricula, string numeroCertificado)
    {
        var usuarioEstudante = await detalhesUsuarioService.ObterUsuarioPorIdAsync(matricula!.Estudante!.UsuarioId) ?? throw new InvalidOperationException($"Estudante {matricula.EstudanteId} não encontrado!");
        var tituloCurso = await cursoAulaService.ObterTituloCursoAsync(matricula.CursoId);
        return $"Certificado de Conclusão\nMatrícula: {matricula.Id}\nEmitido em: {DateTime.Now:yyyy-MM-dd}\nNúmero: {numeroCertificado}\nCurso: {tituloCurso}\nNome do estudante: {usuarioEstudante!.NomeCompleto}";
    }

    private static string GerarNumeroCertificado()
    {
        return $"CERT-{DateTime.Now:yyyyMMddHHmmss}-{Guid.CreateVersion7().ToString("N").Substring(0, 8)}";
    }

    public async Task<IEnumerable<Certificado>> ObterCertificadosDoEstudanteAsync(Guid estudanteId, CancellationToken cancellationToken)
    {
        var estudante = await estudanteRepository.GetByIdAsync(estudanteId);
        if (estudante == null)
            throw new ArgumentException("Estudante não encontrado", nameof(estudanteId));

        var certificados = await estudanteRepository.GetCertificadosByEstudanteIdAsync(estudanteId);
        return certificados;
    }
}