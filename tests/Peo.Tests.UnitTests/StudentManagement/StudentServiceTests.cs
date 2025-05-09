using FluentAssertions;
using Moq;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Interfaces.Services;
using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Application.Services;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Peo.Tests.UnitTests.StudentManagement;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<ICourseLessonService> _courseLessonServiceMock;
    private readonly Mock<IUserDetailsService> _userDetailsServiceMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _courseLessonServiceMock = new Mock<ICourseLessonService>();
        _userDetailsServiceMock = new Mock<IUserDetailsService>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _studentService = new StudentService(
            _studentRepositoryMock.Object,
            _courseLessonServiceMock.Object,
            _userDetailsServiceMock.Object,
            _appIdentityUserMock.Object);
    }

    [Fact]
    public async Task CreateStudentAsync_ShouldCreateAndReturnNewStudent()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var expectedStudent = new Student(userId);
        _studentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.CreateStudentAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        _studentRepositoryMock.Verify(x => x.AddAsync(It.Is<Student>(s => s.UserId == userId)), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnrollStudentAsync_ShouldCreateEnrollmentWhenValid()
    {
        // Arrange
        var studentId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var student = new Student(Guid.CreateVersion7());
        var expectedEnrollment = new Enrollment(studentId, courseId);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);
        _courseLessonServiceMock.Setup(x => x.CheckIfCourseExistsAsync(courseId))
            .ReturnsAsync(true);
        _studentRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync(false);
        _studentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.EnrollStudentAsync(studentId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.StudentId.Should().Be(studentId);
        result.CourseId.Should().Be(courseId);
        _studentRepositoryMock.Verify(x => x.AddEnrollmentAsync(It.Is<Enrollment>(e => e.StudentId == studentId && e.CourseId == courseId)), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnrollStudentAsync_ShouldThrowWhenStudentNotFound()
    {
        // Arrange
        var studentId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.EnrollStudentAsync(studentId, courseId));
    }

    [Fact]
    public async Task EnrollStudentAsync_ShouldThrowWhenCourseNotFound()
    {
        // Arrange
        var studentId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var student = new Student(Guid.CreateVersion7());

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);
        _courseLessonServiceMock.Setup(x => x.CheckIfCourseExistsAsync(courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.EnrollStudentAsync(studentId, courseId));
    }

    [Fact]
    public async Task StartLessonAsync_ShouldCreateProgressWhenValid()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, Guid.CreateVersion7());
        var student = new Student(currentUserId) { Id = studentId }; // Set the student's ID to match enrollment's StudentId

        enrollment.PaymentDone();

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);
        _studentRepositoryMock.Setup(x => x.GetEnrollmentProgressAsync(enrollmentId, lessonId))
            .ReturnsAsync((EnrollmentProgress?)null);
        _studentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.StartLessonAsync(enrollmentId, lessonId);

        // Assert
        result.Should().NotBeNull();
        result.EnrollmentId.Should().Be(enrollmentId);
        result.LessonId.Should().Be(lessonId);
        result.IsCompleted.Should().BeFalse();
        _studentRepositoryMock.Verify(x => x.AddEnrollmentProgressAsync(It.Is<EnrollmentProgress>(p => p.EnrollmentId == enrollmentId && p.LessonId == lessonId)), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EndLessonAsync_ShouldUpdateProgressWhenValid()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, Guid.CreateVersion7());
        var progress = new EnrollmentProgress(enrollmentId, lessonId);
        var student = new Student(currentUserId) { Id = studentId }; // Set the student's ID to match enrollment's StudentId

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);
        _studentRepositoryMock.Setup(x => x.GetEnrollmentProgressAsync(enrollmentId, lessonId))
            .ReturnsAsync(progress);
        _courseLessonServiceMock.Setup(x => x.GetTotalLessonsInCourseAsync(enrollment.CourseId))
            .ReturnsAsync(10);
        _studentRepositoryMock.Setup(x => x.GetCompletedLessonsCountAsync(enrollmentId))
            .ReturnsAsync(9);
        _studentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.EndLessonAsync(enrollmentId, lessonId);

        // Assert
        result.Should().NotBeNull();
        result.EnrollmentId.Should().Be(enrollmentId);
        result.LessonId.Should().Be(lessonId);
        result.IsCompleted.Should().BeTrue();
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentProgressAsync(It.Is<EnrollmentProgress>(p => p.EnrollmentId == enrollmentId && p.LessonId == lessonId && p.IsCompleted)), Times.Once);
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentAsync(It.IsAny<Enrollment>()), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EndLessonAsync_ShouldThrowWhenCurrentUserIsNotTheEnrollmentStudent()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var enrollmentStudentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var differentStudentId = Guid.CreateVersion7();
        var enrollment = new Enrollment(enrollmentStudentId, Guid.CreateVersion7());
        var student = new Student(currentUserId) { Id = differentStudentId }; // Different student ID than enrollment's

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _studentService.EndLessonAsync(enrollmentId, lessonId));
    }

    [Fact]
    public async Task StartLessonAsync_ShouldThrowWhenCurrentUserIsNotTheEnrollmentStudent()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var enrollmentStudentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var differentStudentId = Guid.CreateVersion7();
        var enrollment = new Enrollment(enrollmentStudentId, Guid.CreateVersion7());
        var student = new Student(currentUserId) { Id = differentStudentId }; // Different student ID than enrollment's

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _studentService.StartLessonAsync(enrollmentId, lessonId));
    }

    [Fact]
    public async Task EnrollStudentWithUserIdAsync_ShouldCreateStudentAndEnrollWhenValid()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var student = new Student(userId) { Id = studentId };
        var expectedEnrollment = new Enrollment(studentId, courseId);

        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((Student?)null);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(student);

        _studentRepositoryMock.Setup(x => x.AddAsync(It.Is<Student>(s => s.UserId == userId)))
            .Returns(Task.CompletedTask);

        _courseLessonServiceMock.Setup(x => x.CheckIfCourseExistsAsync(courseId))
            .ReturnsAsync(true);

        _studentRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync(false);

        _studentRepositoryMock.Setup(x => x.AddEnrollmentAsync(It.IsAny<Enrollment>()))
            .Returns(Task.CompletedTask);

        _studentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.EnrollStudentWithUserIdAsync(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.CourseId.Should().Be(courseId);
        _studentRepositoryMock.Verify(x => x.AddAsync(It.Is<Student>(s => s.UserId == userId)), Times.Once);
        _studentRepositoryMock.Verify(x => x.AddEnrollmentAsync(It.Is<Enrollment>(e => e.CourseId == courseId)), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task GetCourseOverallProgress_ShouldReturnProgressPercentage()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, Guid.CreateVersion7());
        var student = new Student(currentUserId) { Id = studentId };
        enrollment.UpdateProgress(75);

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);

        // Act
        var result = await _studentService.GetCourseOverallProgress(enrollmentId);

        // Assert
        result.Should().Be(75);
    }

    [Fact]
    public async Task CompleteEnrollmentAsync_ShouldCompleteEnrollmentAndGenerateCertificate()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var student = new Student(currentUserId) { Id = studentId };
        var enrollment = new Enrollment(studentId, courseId);
        enrollment.PaymentDone();
        enrollment.Student = student;

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);
        _courseLessonServiceMock.Setup(x => x.GetTotalLessonsInCourseAsync(courseId))
            .ReturnsAsync(10);
        _studentRepositoryMock.Setup(x => x.GetCompletedLessonsCountAsync(enrollmentId))
            .ReturnsAsync(10);
        _courseLessonServiceMock.Setup(x => x.GetCourseTitleAsync(courseId))
            .ReturnsAsync("Test Course");
        _userDetailsServiceMock.Setup(x => x.GetUserByIdAsync(currentUserId))
            .ReturnsAsync(new User(currentUserId, "Test User", "test@example.com"));
        _studentRepositoryMock.Setup(x => x.UpdateEnrollmentAsync(It.IsAny<Enrollment>()))
            .Returns(Task.CompletedTask);
        _studentRepositoryMock.Setup(x => x.AddCertificateAsync(It.IsAny<Certificate>()))
            .Returns(Task.CompletedTask);
        _studentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _studentService.CompleteEnrollmentAsync(enrollmentId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(EnrollmentStatus.Completed);
        result.CompletionDate.Should().NotBeNull();
        result.ProgressPercentage.Should().Be(100);
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentAsync(It.Is<Enrollment>(e => e.Status == EnrollmentStatus.Completed)), Times.Once);
        _studentRepositoryMock.Verify(x => x.AddCertificateAsync(It.IsAny<Certificate>()), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CompleteEnrollmentAsync_ShouldThrowWhenNotAllLessonsCompleted()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var currentUserId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, courseId);
        var student = new Student(currentUserId) { Id = studentId };
        enrollment.PaymentDone();

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _studentRepositoryMock.Setup(x => x.GetByUserIdAsync(currentUserId))
            .ReturnsAsync(student);
        _courseLessonServiceMock.Setup(x => x.GetTotalLessonsInCourseAsync(courseId))
            .ReturnsAsync(10);
        _studentRepositoryMock.Setup(x => x.GetCompletedLessonsCountAsync(enrollmentId))
            .ReturnsAsync(8);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _studentService.CompleteEnrollmentAsync(enrollmentId));
    }

    [Fact]
    public async Task GetStudentCertificatesAsync_ShouldReturnCertificates()
    {
        // Arrange
        var studentId = Guid.CreateVersion7();
        var enrollmentId = Guid.CreateVersion7();
        var certificates = new List<Certificate>
        {
            new Certificate(enrollmentId, "Certificate 1", DateTime.Now, "CERT-001"),
            new Certificate(enrollmentId, "Certificate 2", DateTime.Now, "CERT-002")
        };

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(new Student(Guid.CreateVersion7()));
        _studentRepositoryMock.Setup(x => x.GetCertificatesByStudentIdAsync(studentId))
            .ReturnsAsync(certificates);

        // Act
        var result = await _studentService.GetStudentCertificatesAsync(studentId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(certificates);
    }

    [Fact]
    public async Task GetStudentCertificatesAsync_ShouldThrowWhenStudentNotFound()
    {
        // Arrange
        var studentId = Guid.CreateVersion7();
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentCertificatesAsync(studentId, CancellationToken.None));
    }
}