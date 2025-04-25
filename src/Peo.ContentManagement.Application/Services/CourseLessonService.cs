using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Interfaces.Data;
using Peo.Core.Interfaces.Services.Acls;

namespace Peo.ContentManagement.Application.Services;

public class CourseLessonService : ICourseLessonService
{
    private readonly IRepository<Course> _courseRepository;

    public CourseLessonService(IRepository<Course> courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<bool> CheckIfCourseExistsAsync(Guid courseId)
    {
        return await _courseRepository.AnyAsync(c => c.Id == courseId);
    }

    public async Task<decimal> GetCoursePriceAsync(Guid courseId)
    {
        var course = await _courseRepository.GetAsync(courseId);
        return course?.Price ?? 0;
    }

    public async Task<string?> GetCourseTitleAsync(Guid courseId)
    {
        var course = await _courseRepository.GetAsync(courseId);
        return course?.Title;
    }

    public async Task<int> GetTotalLessonsInCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetAsync(courseId);
        return course?.Lessons.Count ?? 0;
    }
}