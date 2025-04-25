using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.StudentManagement.Application.Commands.Enrollment;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.Tests.UnitTests.StudentManagement;

public class CompleteEnrollmentCommandHandlerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<ILogger<CompleteEnrollmentCommandHandler>> _loggerMock;
    private readonly CompleteEnrollmentCommandHandler _handler;

    public CompleteEnrollmentCommandHandlerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _loggerMock = new Mock<ILogger<CompleteEnrollmentCommandHandler>>();
        _handler = new CompleteEnrollmentCommandHandler(
            _studentServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEnrollment_WhenValid()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, courseId);
        enrollment.PaymentDone();
        enrollment.Complete();

        _studentServiceMock.Setup(x => x.CompleteEnrollmentAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var request = new CompleteEnrollmentRequest
        {
            EnrollmentId = enrollmentId
        };
        var command = new CompleteEnrollmentCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Status.Should().Be(EnrollmentStatus.Completed.ToString());
        result.Value.CompletionDate.Should().Be(enrollment.CompletionDate);
        result.Value.OverallProgress.Should().Be(100);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenErrorOccurs()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var errorMessage = "An error occurred";

        _studentServiceMock.Setup(x => x.CompleteEnrollmentAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        var request = new CompleteEnrollmentRequest
        {
            EnrollmentId = enrollmentId
        };
        var command = new CompleteEnrollmentCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }
}