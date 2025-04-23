using Peo.Core.Entities.Base;

namespace Peo.StudentManagement.Domain.Entities;

public class EnrollmentProgress : EntityBase
{
    public Guid EnrollmentId { get; private set; }
    public Guid LessonId { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public bool IsCompleted => CompletedAt.HasValue;

    protected EnrollmentProgress()
    { }

    public EnrollmentProgress(Guid studentId, Guid lessonId)
    {
        EnrollmentId = studentId;
        LessonId = lessonId;
        StartedAt = DateTime.Now;
    }

    public void MarkAsCompleted()
    {
        CompletedAt = DateTime.Now;
    }
}