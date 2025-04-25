using FluentAssertions;
using Moq;
using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.Entities;
using Peo.Billing.Domain.Interfaces.Services;
using Peo.Billing.Domain.ValueObjects;
using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Application.Commands.EnrollmentPayment;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.Tests.UnitTests.StudentManagement;

public class EnrollmentPaymentCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<ICourseLessonService> _courseLessonServiceMock;
    private readonly EnrollmentPaymentCommandHandler _handler;

    public EnrollmentPaymentCommandHandlerTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _courseLessonServiceMock = new Mock<ICourseLessonService>();
        _handler = new EnrollmentPaymentCommandHandler(
            _studentRepositoryMock.Object,
            _paymentServiceMock.Object,
            _courseLessonServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPayment_WhenValid()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, courseId);
        var amount = 99.99m;
        var creditCard = new CreditCard("1234567890123456", "Test User", "12/25", "123");
        var payment = new Payment(enrollmentId, amount);
        payment.ProcessPayment(Guid.CreateVersion7().ToString());
        payment.ConfirmPayment(new CreditCardData { Hash = "hash-123" });

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _courseLessonServiceMock.Setup(x => x.GetCoursePriceAsync(courseId))
            .ReturnsAsync(amount);
        _paymentServiceMock.Setup(x => x.ProcessEnrollmentPaymentAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CreditCard>()))
            .ReturnsAsync(payment);

        var request = new EnrollmentPaymentRequest(enrollmentId, creditCard);
        var command = new EnrollmentPaymentCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PaymentId.Should().Be(payment.Id);
        result.Value.EnrollmentId.Should().Be(enrollmentId);
        result.Value.Amount.Should().Be(amount);
        result.Value.Status.Should().Be(PaymentStatus.Paid);
        result.Value.PaymentDate.Should().Be(payment.PaymentDate);
        result.Value.TransactionId.Should().Be(payment.TransactionId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenErrorOccurs()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var studentId = Guid.CreateVersion7();
        var courseId = Guid.CreateVersion7();
        var enrollment = new Enrollment(studentId, courseId);
        var amount = 99.99m;
        var creditCard = new CreditCard("1234567890123456", "Test User", "12/25", "123");
        var errorMessage = "Payment processing failed";

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _courseLessonServiceMock.Setup(x => x.GetCoursePriceAsync(courseId))
            .ReturnsAsync(amount);
        _paymentServiceMock.Setup(x => x.ProcessEnrollmentPaymentAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CreditCard>()))
            .ThrowsAsync(new Exception(errorMessage));

        var request = new EnrollmentPaymentRequest(enrollmentId, creditCard);
        var command = new EnrollmentPaymentCommand(request);

        // Act & assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}