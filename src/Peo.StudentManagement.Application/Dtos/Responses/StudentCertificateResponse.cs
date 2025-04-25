namespace Peo.StudentManagement.Application.Dtos.Responses;

public record StudentCertificateResponse(
    Guid CertificateId,
    Guid EnrollmentId,
    string Content,
    DateTime? IssueDate,
    string? CertificateNumber);