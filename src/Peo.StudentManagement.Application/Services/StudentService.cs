using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseLessonService _courseLessonService;
    private readonly IUserDetailsService _userDetailsService;

    public StudentService(IStudentRepository studentRepository, ICourseLessonService courseLessonService, IUserDetailsService userDetailsService)
    {
        _studentRepository = studentRepository;
        _courseLessonService = courseLessonService;
        _userDetailsService = userDetailsService;
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

        var courseExists = await _courseLessonService.CheckIfCourseExistsAsync(courseId);

        if (!courseExists)
            throw new ArgumentException("Course not found", nameof(courseId));

        var courseExistsForStudent = await _studentRepository.AnyAsync(s => s.Id == studentId && s.Enrollments.Any(e => e.CourseId == courseId));

        if (courseExistsForStudent)
        {
            throw new ArgumentException("Student is alread enrolled in this course", nameof(courseId));
        }

        var enrollment = new Enrollment(studentId, courseId);
        await _studentRepository.AddEnrollmentAsync(enrollment);
        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);
        return enrollment;
    }

    public async Task<Enrollment> EnrollStudentWithUserIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        student ??= await CreateStudentAsync(userId, cancellationToken);

        return await EnrollStudentAsync(student.Id, courseId, cancellationToken);
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

        var certNumber = GenerateCertificateNumber();

        var certificate = new Certificate(
            enrollmentId: enrollment.Id,
            content: await GenerateCertificateContentAsync(enrollment, certNumber),
            issueDate: DateTime.Now,
            certificateNumber: certNumber
        );
        await _studentRepository.AddCertificateAsync(certificate);

        await _studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return enrollment;
    }

    private async Task<string> GenerateCertificateContentAsync(Enrollment enrollment, string certNumber)
    {
        var studentUser = (await _userDetailsService.GetUserByIdAsync(enrollment.StudentId)) ?? throw new InvalidOperationException($"Student {enrollment.StudentId} not found!");
        var courseTitle = await _courseLessonService.GetCourseTitleAsync(enrollment.CourseId);
        return $"Certificate of Completion\nEnrollment ID: {enrollment.Id}\nIssued on: {DateTime.Now:yyyy-MM-dd}\nNumber: {certNumber}\nCourse: {courseTitle}\nStudent name: {studentUser!.FullName}";
    }

    private static string GenerateCertificateNumber()
    {
        return $"CERT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    }
}