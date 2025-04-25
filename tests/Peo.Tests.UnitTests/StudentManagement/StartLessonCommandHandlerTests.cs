using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.StudentManagement.Application.Commands.Lesson;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.Tests.UnitTests.StudentManagement;

public class StartLessonCommandHandlerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<ILogger<StartLessonCommandHandler>> _loggerMock;
    private readonly StartLessonCommandHandler _handler;

    public StartLessonCommandHandlerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _loggerMock = new Mock<ILogger<StartLessonCommandHandler>>();
        _handler = new StartLessonCommandHandler(
            _studentServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProgress_WhenValid()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var progress = new EnrollmentProgress(enrollmentId, lessonId);

        _studentServiceMock.Setup(x => x.StartLessonAsync(enrollmentId, lessonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progress);
        _studentServiceMock.Setup(x => x.GetCourseOverallProgress(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var request = new StartLessonRequest
        {
            EnrollmentId = enrollmentId,
            LessonId = lessonId
        };
        var command = new StartLessonCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EnrollmentId.Should().Be(enrollmentId);
        result.Value.LessonId.Should().Be(lessonId);
        result.Value.IsCompleted.Should().BeFalse();
        result.Value.StartedAt.Should().Be(progress.StartedAt);
        result.Value.CompletedAt.Should().BeNull();
        result.Value.CourseOverallProgress.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenErrorOccurs()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var errorMessage = "An error occurred";

        _studentServiceMock.Setup(x => x.StartLessonAsync(enrollmentId, lessonId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        var request = new StartLessonRequest
        {
            EnrollmentId = enrollmentId,
            LessonId = lessonId
        };
        var command = new StartLessonCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }
}