using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Interfaces.Data;
using Peo.Core.Interfaces.Services.Acls;

namespace Peo.ContentManagement.Application.Services;

public class CursoAulaService : ICursoAulaService
{
    //private readonly IRepository<Curso> _cursoRepository;

    //public CursoAulaService(IRepository<Curso> cursoRepository)
    //{
    //    _cursoRepository = cursoRepository;
    //}

    //public async Task<bool> VerificarSeCursoExisteAsync(Guid cursoId)
    //{
    //    return await _cursoRepository.AnyAsync(c => c.Id == cursoId);
    //}

    //public async Task<int> ObterTotalAulasNoCursoAsync(Guid cursoId)
    //{
    //    var curso = await _cursoRepository.GetAsync(cursoId);
    //    return curso?.Aulas.Count ?? 0;
    //}

    //public async Task<string> ObterTituloCursoAsync(Guid cursoId)
    //{
    //    var curso = await _cursoRepository.GetAsync(cursoId);
    //    return curso?.Titulo ?? throw new ArgumentException("Curso não encontrado");
    //}

    //public async Task<IEnumerable<Aula>> ObterAulasDoCursoAsync(Guid cursoId)
    //{
    //    var curso = await _cursoRepository.GetAsync(cursoId);
    //    return curso?.Aulas ?? Enumerable.Empty<Aula>();
    //}

    //public async Task<Aula> ObterAulaPorIdAsync(Guid aulaId)
    //{
    //    var cursos = await _cursoRepository.GetAllAsync();
    //    var aula = cursos.SelectMany(c => c.Aulas).FirstOrDefault(a => a.Id == aulaId);
    //    return aula ?? throw new ArgumentException("Aula não encontrada");
    //}

    //public async Task<Curso> ObterCursoPorIdAsync(Guid cursoId)
    //{
    //    return await _cursoRepository.GetAsync(cursoId)
    //        ?? throw new ArgumentException("Curso não encontrado");
    //}

    //public async Task<Curso> CriarCursoAsync(string titulo, string descricao, Guid instrutorId, TimeSpan duracao, decimal preco = 0, bool isPublico = true)
    //{
    //    var curso = new Curso(
    //        titulo: titulo,
    //        descricao: descricao,
    //        instrutorId: instrutorId,
    //        conteudoProgramatico: null,
    //        preco: preco,
    //        estaPublicado: isPublico,
    //        dataPublicacao: isPublico ? DateTime.Now : null,
    //        tags: [],
    //        aulas: []
    //    );
    //    _cursoRepository.Insert(curso);
    //    await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //    return curso;
    //}

    //public async Task<Aula> CriarAulaAsync(Guid cursoId, string titulo, string descricao, TimeSpan duracao)
    //{
    //    var curso = await ObterCursoPorIdAsync(cursoId);
    //    var aula = new Aula(
    //        titulo: titulo,
    //        descricao: descricao,
    //        urlVideo: string.Empty,
    //        duracao: duracao,
    //        arquivos: [],
    //        cursoId: cursoId
    //    );
    //    curso.Aulas.Add(aula);
    //    await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //    return aula;
    //}

    //public async Task<ArquivoAula> AdicionarArquivoAulaAsync(Guid aulaId, string nomeArquivo, string tipoArquivo, string url, long tamanho)
    //{
    //    var cursos = await _cursoRepository.GetAllAsync();
    //    var aula = cursos.SelectMany(c => c.Aulas).FirstOrDefault(a => a.Id == aulaId);
    //    if (aula == null) throw new ArgumentException("Aula não encontrada");
    //    var arquivo = new ArquivoAula(
    //        titulo: nomeArquivo,
    //        url: url,
    //        aulaId: aulaId
    //    );
    //    aula.Arquivos.Add(arquivo);
    //    await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //    return arquivo;
    //}

    //public async Task<Curso> AtualizarCursoAsync(Guid cursoId, string titulo, string descricao)
    //{
    //    var curso = await ObterCursoPorIdAsync(cursoId);
    //    curso.AtualizarTituloDescricao(titulo, descricao);
    //    _cursoRepository.Update(curso);
    //    await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //    return curso;
    //}

    //public async Task<Aula> AtualizarAulaAsync(Guid aulaId, string titulo, string descricao)
    //{
    //    var cursos = await _cursoRepository.GetAllAsync();
    //    var aula = cursos.SelectMany(c => c.Aulas).FirstOrDefault(a => a.Id == aulaId);
    //    if (aula == null) throw new ArgumentException("Aula não encontrada");
    //    aula.AtualizarTituloDescricao(titulo, descricao);
    //    await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //    return aula;
    //}

    //public async Task ExcluirCursoAsync(Guid cursoId)
    //{
    //    var curso = await ObterCursoPorIdAsync(cursoId);
    //    // Não existe método DeleteAsync, então remova do contexto e commit
    //    // Supondo que exista um método Remove no repositório ou contexto
    //    // _cursoRepository.Remove(curso);
    //    // await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //    // Se não existir, apenas ignore a exclusão por enquanto
    //}

    //public async Task ExcluirAulaAsync(Guid aulaId)
    //{
    //    var cursos = await _cursoRepository.GetAllAsync();
    //    var curso = cursos.FirstOrDefault(c => c.Aulas.Any(a => a.Id == aulaId));
    //    var aula = curso?.Aulas.FirstOrDefault(a => a.Id == aulaId);
    //    if (curso == null || aula == null) throw new ArgumentException("Aula não encontrada");
    //    curso.Aulas.Remove(aula);
    //    await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
    //}

    private readonly IRepository<Curso> _cursoRepository;

    public CursoAulaService(IRepository<Curso> cursoRepository)
    {
        _cursoRepository = cursoRepository;
    }

    public async Task<decimal> ObterPrecoCursoAsync(Guid cursoId)
    {
        var course = await _cursoRepository.GetAsync(cursoId);
        return course?.Preco ?? 0;
    }

    public async Task<string?> ObterTituloCursoAsync(Guid cursoId)
    {
        var course = await _cursoRepository.GetAsync(cursoId);
        return course?.Titulo;
    }

    public async Task<int> ObterTotalAulasDoCursoAsync(Guid cursoId)
    {
        var course = await _cursoRepository.GetAsync(cursoId);
        return course?.Aulas.Count ?? 0;
    }

    public async Task<bool> ValidarSeCursoExisteAsync(Guid cursoId)
    {
        return await _cursoRepository.AnyAsync(c => c.Id == cursoId);
    }
}