using Peo.Core.Interfaces.Data;
using Peo.StudentManagement.Domain.Entities;

namespace Peo.StudentManagement.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByIdAsync(Guid id);
    Task AddAsync(Student student);
    Task AddEnrollmentAsync(Enrollment enrollment);
} 