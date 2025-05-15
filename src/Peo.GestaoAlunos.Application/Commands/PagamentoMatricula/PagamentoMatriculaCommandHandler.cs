using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Interfaces.Services;
using Peo.Core.Interfaces.Services.Acls;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Application.Commands.PagamentoMatricula;

public class PagamentoMatriculaCommandHandler : IRequestHandler<PagamentoMatriculaCommand, Result<PagamentoMatriculaResponse>>
{
    private readonly IEstudanteRepository _estudanteRepository;
    private readonly IPagamentoService _pagamentoService;
    private readonly ICursoAulaService _aulaCursoService;

    public PagamentoMatriculaCommandHandler(
        IEstudanteRepository estudanteRepository,
        IPagamentoService pagamentoService,
        ICursoAulaService aulaCursoService)
    {
        _estudanteRepository = estudanteRepository;
        _pagamentoService = pagamentoService;
        _aulaCursoService = aulaCursoService;
    }

    public async Task<Result<PagamentoMatriculaResponse>> Handle(PagamentoMatriculaCommand request, CancellationToken cancellationToken)
    {
        var matricula = await _estudanteRepository.GetMatriculaByIdAsync(request.Request.MatriculaId);

        if (matricula is null)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error("Matrícula não encontrada"));
        }

        if (matricula.Status != StatusMatricula.PendentePagamento)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error($"Não é possível processar pagamento para matrícula com status {matricula.Status}"));
        }

        var preco = await _aulaCursoService.ObterPrecoCursoAsync(matricula.CursoId);
                
        var pagamento = await _pagamentoService.ProcessarPagamentoMatriculaAsync(
            matricula.Id,
            preco,
            request.Request.DadosCartao
        );

        var response = new PagamentoMatriculaResponse(
            pagamento.MatriculaId,
            pagamento.Status.ToString(),
            pagamento.Valor
        );

        if (pagamento.Status == Faturamento.Domain.ValueObjects.StatusPagamento.Falha)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error($"Pagamento falhou: {pagamento.Detalhes}"));
        }

        return Result.Success(response);
    }
}