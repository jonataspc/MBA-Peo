using Microsoft.Extensions.Logging;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Commands.Matricula;

public class ConcluirMatriculaCommandHandler : IRequestHandler<ConcluirMatriculaCommand, Result<ConcluirMatriculaResponse>>
{
    private readonly IEstudanteService _estudanteService;
    private readonly ILogger<ConcluirMatriculaCommandHandler> _logger;

    public ConcluirMatriculaCommandHandler(IEstudanteService estudanteService, ILogger<ConcluirMatriculaCommandHandler> logger)
    {
        _estudanteService = estudanteService;
        _logger = logger;
    }

    public async Task<Result<ConcluirMatriculaResponse>> Handle(ConcluirMatriculaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _estudanteService.ConcluirMatriculaAsync(request.Request.MatriculaId, cancellationToken);

            var response = new ConcluirMatriculaResponse(enrollment.Id, enrollment.Status.ToString(), enrollment.DataConclusao, enrollment.PercentualProgresso);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao concluir matrícula {MatriculaId}", request.Request.MatriculaId);
            _logger.LogError(ex, "Error completing enrollment {EnrollmentId}", request.Request.MatriculaId);
            return Result.Failure<ConcluirMatriculaResponse>(new Error(ex.Message));
        }
    }
}