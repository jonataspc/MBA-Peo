using Peo.Core.Interfaces.Data;
using Peo.StudentManagement.Domain.Entities;

namespace Peo.StudentManagement.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByIdAsync(Guid id);
    Task AddAsync(Student student);
    Task AddEnrollmentAsync(Enrollment enrollment);
    Task<Enrollment?> GetEnrollmentByIdAsync(Guid enrollmentId);
    Task UpdateEnrollmentAsync(Enrollment enrollment);
    Task AddEnrollmentProgressAsync(EnrollmentProgress progress);
    Task<EnrollmentProgress?> GetEnrollmentProgressAsync(Guid enrollmentId, Guid lessonId);
    Task UpdateEnrollmentProgressAsync(EnrollmentProgress progress);
    Task<int> GetCompletedLessonsCountAsync(Guid enrollmentId);
    Task AddCertificateAsync(Certificate certificate);
} 