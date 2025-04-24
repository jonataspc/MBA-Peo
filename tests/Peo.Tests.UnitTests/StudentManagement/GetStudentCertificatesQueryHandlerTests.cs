using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Application.Queries.GetStudentCertificates;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using Xunit;

namespace Peo.Tests.UnitTests.StudentManagement;

public class GetStudentCertificatesQueryHandlerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<ILogger<GetStudentCertificatesQueryHandler>> _loggerMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly GetStudentCertificatesQueryHandler _handler;

    public GetStudentCertificatesQueryHandlerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _loggerMock = new Mock<ILogger<GetStudentCertificatesQueryHandler>>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _handler = new GetStudentCertificatesQueryHandler(
            _studentServiceMock.Object,
            _loggerMock.Object,
            _appIdentityUserMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCertificates_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var student = new Student(userId) { Id = studentId };
        var certificates = new List<Certificate>
        {
            new Certificate(enrollmentId, "Certificate 1", DateTime.Now, "CERT-001"),
            new Certificate(enrollmentId, "Certificate 2", DateTime.Now, "CERT-002")
        };

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(userId);
        _studentServiceMock.Setup(x => x.GetStudentByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _studentServiceMock.Setup(x => x.GetStudentCertificatesAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificates);

        // Act
        var result = await _handler.Handle(new GetStudentCertificatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(certificates.Select(c => new StudentCertificateResponse(
            c.Id,
            c.EnrollmentId,
            c.Content,
            c.IssueDate,
            c.CertificateNumber
        )));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenStudentNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var errorMessage = "Student not found";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(userId);
        _studentServiceMock.Setup(x => x.GetStudentByUserId(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _handler.Handle(new GetStudentCertificatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenErrorOccurs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var student = new Student(userId) { Id = studentId };
        var errorMessage = "An unexpected error occurred";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(userId);
        _studentServiceMock.Setup(x => x.GetStudentByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _studentServiceMock.Setup(x => x.GetStudentCertificatesAsync(studentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _handler.Handle(new GetStudentCertificatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }
} 