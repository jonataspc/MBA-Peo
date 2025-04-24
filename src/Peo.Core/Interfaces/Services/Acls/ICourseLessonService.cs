namespace Peo.Core.Interfaces.Services.Acls;

public interface ICourseLessonService
{
    Task<int> GetTotalLessonsInCourseAsync(Guid courseId);
} 