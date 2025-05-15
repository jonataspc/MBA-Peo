namespace Peo.StudentManagement.Application.Dtos.Responses;

public record ProgressoAulaResponse(
    Guid MatriculaId,
    Guid AulaId,
    bool EstaConcluida,
    DateTime? DataInicio,
    DateTime? DataConclusao,
    int ProgressoGeralCurso
);