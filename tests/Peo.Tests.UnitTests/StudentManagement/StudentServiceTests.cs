using FluentAssertions;
using Moq;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Application.Services;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;
using System.Linq.Expressions;
using Xunit;

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
        var userId = Guid.NewGuid();
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
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var student = new Student(Guid.NewGuid());
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
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.EnrollStudentAsync(studentId, courseId));
    }

    [Fact]
    public async Task EnrollStudentAsync_ShouldThrowWhenCourseNotFound()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var student = new Student(Guid.NewGuid());

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
        var enrollmentId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var enrollment = new Enrollment(studentId, Guid.NewGuid());
        var currentUserId = studentId;

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
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
        var enrollmentId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var enrollment = new Enrollment(studentId, Guid.NewGuid());
        var progress = new EnrollmentProgress(enrollmentId, lessonId);
        var currentUserId = studentId;

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(currentUserId);
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
        result.IsCompleted.Should().BeTrue();
        result.CompletedAt.Should().NotBeNull();
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentProgressAsync(progress), Times.Once);
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentAsync(enrollment), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
} 