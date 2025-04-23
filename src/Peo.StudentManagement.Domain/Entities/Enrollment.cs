using Peo.Core.Entities.Base;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.StudentManagement.Domain.Entities;

public class Enrollment : EntityBase
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public int ProgressPercentage { get; private set; }

    protected Enrollment() { }

    public Enrollment(Guid studentId, Guid courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
        EnrollmentDate = DateTime.Now;
        Status = EnrollmentStatus.PendingPayment;
        ProgressPercentage = 0;
    }

    public void UpdateProgress(int percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100");

        ProgressPercentage = percentage;
    }

    public void Complete()
    {
        CompletionDate = DateTime.Now;
        Status = EnrollmentStatus.Completed;
        ProgressPercentage = 100;
    }

    public void PaymentDone()
    {
        Status = EnrollmentStatus.Active;
    }

    public void Cancel()
    {
        Status = EnrollmentStatus.Cancelled;
    }
}
