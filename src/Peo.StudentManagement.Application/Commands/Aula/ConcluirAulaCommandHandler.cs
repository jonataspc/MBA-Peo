using Microsoft.Extensions.Logging;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Commands.Aula;

public class ConcluirAulaCommandHandler : IRequestHandler<ConcluirAulaCommand, Result<ProgressoAulaResponse>>
{
    private readonly IEstudanteService _estudanteService;
    private readonly ILogger<ConcluirAulaCommandHandler> _logger;

    public ConcluirAulaCommandHandler(IEstudanteService estudanteService, ILogger<ConcluirAulaCommandHandler> logger)
    {
        _estudanteService = estudanteService;
        _logger = logger;
    }

    public async Task<Result<ProgressoAulaResponse>> Handle(ConcluirAulaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var progresso = await _estudanteService.ConcluirAulaAsync(request.Request.MatriculaId, request.Request.AulaId, cancellationToken);

            var response = new ProgressoAulaResponse(
                progresso.MatriculaId,
                progresso.AulaId,
                progresso.EstaConcluido,
                progresso.DataInicio,
                progresso.DataConclusao,
                await _estudanteService.ObterProgressoGeralCursoAsync(progresso.MatriculaId, cancellationToken)
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao concluir aula para matrícula {MatriculaId} e aula {AulaId}",
                request.Request.MatriculaId, request.Request.AulaId);
            return Result.Failure<ProgressoAulaResponse>(new Error(ex.Message));
        }
    }
}