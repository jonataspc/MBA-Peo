using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.Tests.IntegrationTests.Setup;

public class TestDatabaseSetup
{
    private readonly IStudentRepository _studentRepository;
    private readonly IRepository<Course> _courseRepository;

    private readonly IServiceScope _scope;

    internal readonly string UserTestPassword = "Test123!91726312389831625192JHTBADPDJANDHJPXASDO";
    internal readonly string UserTestEmail = $"{Guid.CreateVersion7()}@example.com";

    public TestDatabaseSetup(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateScope();

        _studentRepository = _scope.ServiceProvider.GetRequiredService<IStudentRepository>();
        _courseRepository = _scope.ServiceProvider.GetRequiredService<IRepository<Course>>();
    }

    public async Task<Student> CreateTestStudentAsync(Guid userId)
    {
        var userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var user = new User(userId, $"John Conor {Random.Shared.NextDouble()}", UserTestEmail);

        IUserRepository userRepo = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        userRepo.Insert(user);
        await userRepo.UnitOfWork.CommitAsync(default);

        //  add user to Identity
        var identityUser = new IdentityUser
        {
            Id = userId.ToString(),
            UserName = user.Email,
            Email = user.Email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(identityUser, UserTestPassword);
        await userManager.AddToRoleAsync(identityUser, AccessRoles.Student);

        var student = new Student(userId);
        await _studentRepository.AddAsync(student);
        await _studentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return student;
    }

    public async Task CreateAdminUser(Guid userId)
    {
        var userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var user = new User(userId, $"Sara Conor {Random.Shared.NextDouble()}", UserTestEmail);

        IUserRepository userRepo = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        userRepo.Insert(user);
        await userRepo.UnitOfWork.CommitAsync(default);

        //  add user to Identity
        var identityUser = new IdentityUser
        {
            Id = userId.ToString(),
            UserName = user.Email,
            Email = user.Email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(identityUser, UserTestPassword);
        await userManager.AddToRoleAsync(identityUser, AccessRoles.Admin);
    }

    public async Task<Enrollment> CreateTestEnrollmentAsync(Guid studentId, Guid courseId, bool makePaymentDone)
    {
        var enrollment = new Enrollment(studentId, courseId);

        if (makePaymentDone)
            enrollment.PaymentDone();

        await _studentRepository.AddEnrollmentAsync(enrollment);
        await _studentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return enrollment;
    }

    public async Task<EnrollmentProgress> CreateTestLessonProgressAsync(Guid enrollmentId, Guid lessonId)
    {
        var progress = new EnrollmentProgress(enrollmentId, lessonId);
        await _studentRepository.AddEnrollmentProgressAsync(progress);
        await _studentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return progress;
    }

    public async Task<Certificate> CreateTestCertificateAsync(Guid enrollmentId, string content)
    {
        var certificate = new Certificate(enrollmentId, content, DateTime.UtcNow, $"CERT-{Guid.CreateVersion7():N}");
        await _studentRepository.AddCertificateAsync(certificate);
        await _studentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return certificate;
    }

    public async Task<Course> CreateTestCourseAsync(
        string title = "Test Course",
        string description = "Test Course Description",
        Guid? instructorId = null,
        decimal price = 99.99m,
        bool isPublished = true)
    {
        instructorId ??= Guid.CreateVersion7();

        var course = new Course(
            title,
            description,
            instructorId.Value,
            null, // No program content for test
            price,
            isPublished,
            isPublished ? DateTime.UtcNow : null,
            new List<string> { "test", "integration" },
            new List<Lesson>
            {
                new Lesson("", "", "", TimeSpan.FromSeconds(10), new List<LessonFile>
                {
                    new LessonFile( "", "", Guid.Empty),
                    new LessonFile( "", "", Guid.Empty),
                    new LessonFile( "", "", Guid.Empty)
                }, Guid.Empty),
                new Lesson("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Lesson("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Lesson("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Lesson("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Lesson("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty)
            }
        );

        _courseRepository.Insert(course);
        await _courseRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return course;
    }

    public async Task CleanupAsync()
    {
        // Add cleanup logic here if needed
        _scope.Dispose();
        await Task.CompletedTask;
    }
}