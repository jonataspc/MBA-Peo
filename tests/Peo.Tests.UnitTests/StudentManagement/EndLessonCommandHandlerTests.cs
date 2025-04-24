using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.StudentManagement.Application.Commands.Lesson;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Xunit;

namespace Peo.Tests.UnitTests.StudentManagement;

public class EndLessonCommandHandlerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<ILogger<EndLessonCommandHandler>> _loggerMock;
    private readonly EndLessonCommandHandler _handler;

    public EndLessonCommandHandlerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _loggerMock = new Mock<ILogger<EndLessonCommandHandler>>();
        _handler = new EndLessonCommandHandler(
            _studentServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProgress_WhenValid()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var startedAt = DateTime.Now.AddHours(-1);
        var completedAt = DateTime.Now;
        var progress = new EnrollmentProgress(enrollmentId, lessonId);
        progress.MarkAsCompleted();

        _studentServiceMock.Setup(x => x.EndLessonAsync(enrollmentId, lessonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progress);
        _studentServiceMock.Setup(x => x.GetCourseOverallProgress(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);

        var request = new EndLessonRequest
        {
            EnrollmentId = enrollmentId,
            LessonId = lessonId
        };
        var command = new EndLessonCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EnrollmentId.Should().Be(enrollmentId);
        result.Value.LessonId.Should().Be(lessonId);
        result.Value.IsCompleted.Should().BeTrue();
        result.Value.StartedAt.Should().Be(progress.StartedAt);
        result.Value.CompletedAt.Should().Be(progress.CompletedAt);
        result.Value.CourseOverallProgress.Should().Be(50);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenErrorOccurs()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var lessonId = Guid.CreateVersion7();
        var errorMessage = "An error occurred";

        _studentServiceMock.Setup(x => x.EndLessonAsync(enrollmentId, lessonId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        var request = new EndLessonRequest
        {
            EnrollmentId = enrollmentId,
            LessonId = lessonId
        };
        var command = new EndLessonCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }
} 