using Peo.StudentManagement.Domain.Entities;

namespace Peo.StudentManagement.Domain.Interfaces
{
    public interface IStudentService
    {
        Task<Student> CreateStudentAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<Enrollment> EnrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);

        Task<EnrollmentProgress> StartLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);

        Task<EnrollmentProgress> EndLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);

        Task<Enrollment> CompleteEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

        Task<Enrollment> EnrollStudentWithUserIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);

        Task<int> GetCourseOverallProgress(Guid enrollmentId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Certificate>> GetStudentCertificatesAsync(Guid studentId, CancellationToken cancellationToken);

        Task<Student> GetStudentByUserId(Guid userId, CancellationToken cancellationToken);
    }
}