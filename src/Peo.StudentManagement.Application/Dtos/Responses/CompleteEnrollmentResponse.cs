namespace Peo.StudentManagement.Application.Dtos.Responses;

public record CompleteEnrollmentResponse(Guid EnrollmentId, string Status, DateTime? CompletionDate, decimal OverallProgress);