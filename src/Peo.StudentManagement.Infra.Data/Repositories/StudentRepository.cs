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
} 