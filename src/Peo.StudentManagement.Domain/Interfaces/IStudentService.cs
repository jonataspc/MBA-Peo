using Peo.StudentManagement.Domain.Entities;

namespace Peo.StudentManagement.Application.Services
{
    public interface IStudentService
    {
        Task<Student> CreateStudentAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Enrollment> EnrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    }
}