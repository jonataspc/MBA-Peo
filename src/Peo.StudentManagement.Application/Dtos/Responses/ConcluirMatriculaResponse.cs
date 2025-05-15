namespace Peo.StudentManagement.Application.Dtos.Responses;

public record ConcluirMatriculaResponse(Guid MatriculaId, string Status, DateTime? DataConclusao, decimal ProgressoGeral);