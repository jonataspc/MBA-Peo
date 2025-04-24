using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseLessonService _courseLessonService;

    public StudentService(IStudentRepository studentRepository, ICourseLessonService courseLessonService)
    {
        _studentRepository = studentRepository;
        _courseLessonService = courseLessonService;
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

    public async Task<EnrollmentProgress> StartLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        if (enrollment.Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Cannot start lesson for inactive enrollment");

        var existingProgress = await _studentRepository.GetEnrollmentProgressAsync(enrollmentId, lessonId);
        if (existingProgress != null)
            throw new InvalidOperationException("Lesson already started");

        var progress = new EnrollmentProgress(enrollmentId, lessonId);
        await _studentRepository.AddEnrollmentProgressAsync(progress);
        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progress;
    }

    public async Task<EnrollmentProgress> EndLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        var progress = await _studentRepository.GetEnrollmentProgressAsync(enrollmentId, lessonId)
            ?? throw new ArgumentException("Lesson not started", nameof(lessonId));

        if (progress.IsCompleted)
            throw new InvalidOperationException("Lesson already completed");

        // Mark lesson as completed
        progress.MarkAsCompleted();
        await _studentRepository.UpdateEnrollmentProgressAsync(progress);

        // Calculate and update overall progress
        var totalLessons = await _courseLessonService.GetTotalLessonsInCourseAsync(enrollment.CourseId);
        var completedLessons = await _studentRepository.GetCompletedLessonsCountAsync(enrollmentId);

        var newProgressPercentage = (int)((completedLessons * 100.0) / totalLessons);
        enrollment.UpdateProgress(newProgressPercentage);

        // If all lessons are completed, mark enrollment as completed
        if (newProgressPercentage == 100)
        {
            enrollment.Complete();
        }

        await _studentRepository.UpdateEnrollmentAsync(enrollment);
        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progress;
    }

    public async Task<Enrollment> CompleteEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        if (enrollment.Status != EnrollmentStatus.Active)
            throw new InvalidOperationException($"Cannot conclude enrollment in {enrollment.Status} status");

        // Check if all lessons are completed
        var totalLessons = await _courseLessonService.GetTotalLessonsInCourseAsync(enrollment.CourseId);
        var completedLessons = await _studentRepository.GetCompletedLessonsCountAsync(enrollmentId);

        if (completedLessons < totalLessons)
            throw new InvalidOperationException($"Cannot conclude enrollment. {completedLessons} of {totalLessons} lessons completed.");

        enrollment.Complete();
        await _studentRepository.UpdateEnrollmentAsync(enrollment);
        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return enrollment;
    }
}