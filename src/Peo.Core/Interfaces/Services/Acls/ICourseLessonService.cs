namespace Peo.Core.Interfaces.Services.Acls;

public interface ICourseLessonService
{
    Task<int> GetTotalLessonsInCourseAsync(Guid courseId);

    Task<string?> GetCourseTitleAsync(Guid courseId);

    Task<bool> CheckIfCourseExistsAsync(Guid courseId);

    Task<decimal> GetCoursePriceAsync(Guid courseId);
}