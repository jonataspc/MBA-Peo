using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.StudentManagement.Application.Commands.CourseEnrollment;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Xunit;

namespace Peo.Tests.UnitTests.StudentManagement;

public class CourseEnrollmentCommandHandlerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly Mock<ILogger<CourseEnrollmentCommandHandler>> _loggerMock;
    private readonly CourseEnrollmentCommandHandler _handler;

    public CourseEnrollmentCommandHandlerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _loggerMock = new Mock<ILogger<CourseEnrollmentCommandHandler>>();
        _handler = new CourseEnrollmentCommandHandler(
            _studentServiceMock.Object,
            _appIdentityUserMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEnrollmentId_WhenValid()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var enrollmentId = Guid.CreateVersion7();
        var enrollment = new Enrollment(Guid.CreateVersion7(), courseId) { Id = enrollmentId };

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(userId);

        _appIdentityUserMock.Setup(x => x.GetUsername())
            .Returns("John Doe");

        _appIdentityUserMock.Setup(x => x.IsAuthenticated())
            .Returns(true);

        _appIdentityUserMock.Setup(x => x.IsInRole(It.IsAny<string>()))
            .Returns(true);

        _appIdentityUserMock.Setup(x => x.IsAdmin())
            .Returns(true);

        _appIdentityUserMock.Setup(x => x.GetLocalIpAddress())
            .Returns("127.0.0.1");

        _appIdentityUserMock.Setup(x => x.GetRemoteIpAddress())
            .Returns("127.0.0.1");

        _studentServiceMock.Setup(x => x.EnrollStudentWithUserIdAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var request = new CourseEnrollmentRequest(courseId);
        var command = new CourseEnrollmentCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EnrollmentId.Should().Be(enrollmentId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenErrorOccurs()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var errorMessage = "An error occurred";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(userId);
        _studentServiceMock.Setup(x => x.EnrollStudentWithUserIdAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        var request = new CourseEnrollmentRequest(courseId);
        var command = new CourseEnrollmentCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }
} 