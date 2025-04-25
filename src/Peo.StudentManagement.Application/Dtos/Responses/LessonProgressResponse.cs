namespace Peo.StudentManagement.Application.Dtos.Responses;

public record LessonProgressResponse(Guid EnrollmentId, Guid LessonId, bool IsCompleted, DateTime? StartedAt, DateTime? CompletedAt, int CourseOverallProgress);