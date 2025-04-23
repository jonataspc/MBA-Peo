using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Enums;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Services;

public class StudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student> CreateStudentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var student = new Student(userId);
        await _studentRepository.AddAsync(student);
        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);
        return student;
    }

    public async Task<Enrollment> EnrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new ArgumentException("Student not found", nameof(studentId));

        var enrollment = new Enrollment(studentId, courseId);
        await _studentRepository.AddEnrollmentAsync(enrollment);
        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);
        return enrollment;
    }
} 