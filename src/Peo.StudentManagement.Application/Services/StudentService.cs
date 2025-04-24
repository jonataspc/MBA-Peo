using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.StudentManagement.Application.Services;

public class StudentService(
    IStudentRepository studentRepository,
    ICourseLessonService courseLessonService,
    IUserDetailsService userDetailsService,
    IAppIdentityUser appIdentityUser) : IStudentService
{
    public async Task<Student> CreateStudentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var student = new Student(userId);
        await studentRepository.AddAsync(student);
        await studentRepository.UnitOfWork.CommitAsync(cancellationToken);
        return student;
    }

    public async Task<Enrollment> EnrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new ArgumentException("Student not found", nameof(studentId));

        var courseExists = await courseLessonService.CheckIfCourseExistsAsync(courseId);

        if (!courseExists)
            throw new ArgumentException("Course not found", nameof(courseId));

        var courseExistsForStudent = await studentRepository.AnyAsync(s => s.Id == studentId && s.Enrollments.Any(e => e.CourseId == courseId));

        if (courseExistsForStudent)
        {
            throw new ArgumentException("Student is alread enrolled in this course", nameof(courseId));
        }

        var enrollment = new Enrollment(studentId, courseId);
        await studentRepository.AddEnrollmentAsync(enrollment);
        await studentRepository.UnitOfWork.CommitAsync(cancellationToken);
        return enrollment;
    }

    public async Task<Enrollment> EnrollStudentWithUserIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        Student student = await GetStudentByUserId(userId, cancellationToken);

        return await EnrollStudentAsync(student.Id, courseId, cancellationToken);
    }

    public async Task<Student> GetStudentByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByUserIdAsync(userId);

        student ??= await CreateStudentAsync(userId, cancellationToken);
        return student;
    }

    public async Task<EnrollmentProgress> StartLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        var enrollment = await studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        await ValidateStudentIsTheCurrentLoggedUser(enrollment, cancellationToken);

        if (enrollment.Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Cannot start lesson for inactive enrollment");

        var existingProgress = await studentRepository.GetEnrollmentProgressAsync(enrollmentId, lessonId);
        if (existingProgress != null)
            throw new InvalidOperationException("Lesson already started");

        var progress = new EnrollmentProgress(enrollmentId, lessonId);
        await studentRepository.AddEnrollmentProgressAsync(progress);
        await studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progress;
    }

    private async Task ValidateStudentIsTheCurrentLoggedUser(Enrollment enrollment, CancellationToken cancellationToken)
    {
        var student = await GetStudentByUserId(appIdentityUser.GetUserId(), cancellationToken);

        if (student.Id != enrollment.StudentId)
        {
            throw new DomainException("Isolation violation was detected (current user is not the student of the enrollment");
        }
    }

    public async Task<EnrollmentProgress> EndLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        var enrollment = await studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        await ValidateStudentIsTheCurrentLoggedUser(enrollment, cancellationToken);

        var progress = await studentRepository.GetEnrollmentProgressAsync(enrollmentId, lessonId)
            ?? throw new ArgumentException("Lesson not started", nameof(lessonId));

        if (progress.IsCompleted)
            throw new InvalidOperationException("Lesson already completed");

        // Mark lesson as completed
        progress.MarkAsCompleted();
        await studentRepository.UpdateEnrollmentProgressAsync(progress);

        // Calculate and update overall progress
        var totalLessons = await courseLessonService.GetTotalLessonsInCourseAsync(enrollment.CourseId);
        var completedLessons = await studentRepository.GetCompletedLessonsCountAsync(enrollmentId);

        var newProgressPercentage = (int)((completedLessons * 100.0) / totalLessons);
        enrollment.UpdateProgress(newProgressPercentage);

        // If all lessons are completed, mark enrollment as completed
        if (newProgressPercentage == 100)
        {
            enrollment.Complete();
        }

        await studentRepository.UpdateEnrollmentAsync(enrollment);
        await studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progress;
    }

    public async Task<int> GetCourseOverallProgress(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        await ValidateStudentIsTheCurrentLoggedUser(enrollment, cancellationToken);

        return enrollment.ProgressPercentage;
    }

    public async Task<Enrollment> CompleteEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new ArgumentException("Enrollment not found", nameof(enrollmentId));

        await ValidateStudentIsTheCurrentLoggedUser(enrollment, cancellationToken);

        if (enrollment.Status != EnrollmentStatus.Active)
            throw new InvalidOperationException($"Cannot conclude enrollment in {enrollment.Status} status");

        // Check if all lessons are completed
        var totalLessons = await courseLessonService.GetTotalLessonsInCourseAsync(enrollment.CourseId);
        var completedLessons = await studentRepository.GetCompletedLessonsCountAsync(enrollmentId);

        if (completedLessons < totalLessons)
            throw new InvalidOperationException($"Cannot conclude enrollment. {completedLessons} of {totalLessons} lessons completed.");

        enrollment.Complete();
        await studentRepository.UpdateEnrollmentAsync(enrollment);

        var certNumber = GenerateCertificateNumber();

        var certificate = new Certificate(
            enrollmentId: enrollment.Id,
            content: await GenerateCertificateContentAsync(enrollment, certNumber),
            issueDate: DateTime.Now,
            certificateNumber: certNumber
        );
        await studentRepository.AddCertificateAsync(certificate);

        await studentRepository.UnitOfWork.CommitAsync(cancellationToken);

        return enrollment;
    }

    private async Task<string> GenerateCertificateContentAsync(Enrollment enrollment, string certNumber)
    {
        var studentUser = (await userDetailsService.GetUserByIdAsync(enrollment!.Student!.UserId)) ?? throw new InvalidOperationException($"Student {enrollment.StudentId} not found!");
        var courseTitle = await courseLessonService.GetCourseTitleAsync(enrollment.CourseId);
        return $"Certificate of Completion\nEnrollment ID: {enrollment.Id}\nIssued on: {DateTime.Now:yyyy-MM-dd}\nNumber: {certNumber}\nCourse: {courseTitle}\nStudent name: {studentUser!.FullName}";
    }

    private static string GenerateCertificateNumber()
    {
        return $"CERT-{DateTime.Now:yyyyMMdd}-{Guid.CreateVersion7().ToString("N").Substring(0, 8)}";
    }

    public async Task<IEnumerable<Certificate>> GetStudentCertificatesAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new ArgumentException("Student not found", nameof(studentId));

        var certificates = await studentRepository.GetCertificatesByStudentIdAsync(studentId);
        return certificates;
    }
}