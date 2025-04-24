using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Repositories;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Infra.Data.Contexts;

namespace Peo.StudentManagement.Infra.Data.Repositories;

public class StudentRepository : GenericRepository<Student, StudentManagementContext>, IStudentRepository
{
    public StudentRepository(StudentManagementContext context) : base(context)
    {
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await GetAsync(id);
    }

    public async Task AddAsync(Student student)
    {
        await AddRangeAsync(new[] { student });
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        await _dbContext.Enrollments.AddAsync(enrollment);
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(Guid enrollmentId)
    {
        return await _dbContext.Enrollments.FindAsync(enrollmentId);
    }

    public Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        _dbContext.Enrollments.Update(enrollment);
        return Task.CompletedTask;
    }

    public async Task AddEnrollmentProgressAsync(EnrollmentProgress progress)
    {
        await _dbContext.EnrollmentProgresses.AddAsync(progress);
    }

    public async Task<EnrollmentProgress?> GetEnrollmentProgressAsync(Guid enrollmentId, Guid lessonId)
    {
        return await _dbContext.EnrollmentProgresses
            .FirstOrDefaultAsync(ep => ep.EnrollmentId == enrollmentId && ep.LessonId == lessonId);
    }

    public Task UpdateEnrollmentProgressAsync(EnrollmentProgress progress)
    {
        _dbContext.EnrollmentProgresses.Update(progress);
        return Task.CompletedTask;
    }

    public async Task<int> GetCompletedLessonsCountAsync(Guid enrollmentId)
    {
        return await _dbContext.EnrollmentProgresses
            .CountAsync(ep => ep.EnrollmentId == enrollmentId && ep.IsCompleted);
    }

    public async Task AddCertificateAsync(Certificate certificate)
    {
        await _dbContext.Certificates.AddAsync(certificate);
    }
}