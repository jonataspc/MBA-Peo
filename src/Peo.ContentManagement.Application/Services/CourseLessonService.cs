using Microsoft.EntityFrameworkCore;
using Peo.ContentManagement.Infra.Data.Contexts;
using Peo.Core.Interfaces.Services.Acls;

namespace Peo.ContentManagement.Application.Services;

public class CourseLessonService : ICourseLessonService
{
    private readonly ContentManagementContext _context;

    public CourseLessonService(ContentManagementContext context)
    {
        _context = context;
    }

    public async Task<string?> GetCourseTitleAsync(Guid courseId)
    {
        return await _context.Courses
            .Where(c => c.Id == courseId)
            .Select(c => c.Title)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetTotalLessonsInCourseAsync(Guid courseId)
    {
        return await _context.Lessons
            .CountAsync(l => l.CourseId == courseId);
    }
} 