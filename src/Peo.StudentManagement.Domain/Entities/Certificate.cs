using Peo.Core.Entities.Base;

namespace Peo.StudentManagement.Domain.Entities;

public class Certificate : EntityBase
{
    public Guid EnrollmentId { get; private set; }
    public string Content { get; private set; } = null!;
    public DateTime? IssueDate { get; private set; }
    public string? CertificateNumber { get; private set; }
    public virtual Enrollment Enrollment { get; private set; } = null!;

    public Certificate()
    {
    }

    public Certificate(Guid enrollmentId, string content, DateTime? issueDate, string? certificateNumber)
    {
        EnrollmentId = enrollmentId;
        Content = content;
        IssueDate = issueDate;
        CertificateNumber = certificateNumber;
    }
}