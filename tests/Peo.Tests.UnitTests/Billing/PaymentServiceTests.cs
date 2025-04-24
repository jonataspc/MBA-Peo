using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Peo.Billing.Application.Services;
using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.Entities;
using Peo.Billing.Domain.Interfaces.Brokers;
using Peo.Billing.Domain.Interfaces.Services;
using Peo.Billing.Domain.ValueObjects;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Data;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace Peo.Tests.UnitTests.Billing;

public class PaymentServiceTests
{
    private readonly Mock<IRepository<Payment>> _paymentRepositoryMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IPaymentBrokerService> _paymentBrokerServiceMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IRepository<Payment>>();
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _paymentBrokerServiceMock = new Mock<IPaymentBrokerService>();
        _paymentService = new PaymentService(
            _paymentRepositoryMock.Object,
            _studentRepositoryMock.Object,
            _paymentBrokerServiceMock.Object);
    }

    [Fact]
    public async Task ProcessEnrollmentPaymentAsync_ShouldProcessPaymentSuccessfully()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var amount = 99.99m;
        var creditCard = new CreditCard("1234567890123456", "Test User", "12/25", "123");
        var enrollment = new Enrollment(Guid.CreateVersion7(), Guid.CreateVersion7());
        var payment = new Payment(enrollmentId, amount);
        var brokerResult = new PaymentBrokerResult(true, null, "hash-123");
        var studentUnitOfWorkMock = new Mock<IUnitOfWork>();

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _studentRepositoryMock.Setup(x => x.UnitOfWork)
            .Returns(studentUnitOfWorkMock.Object);
        studentUnitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentBrokerServiceMock.Setup(x => x.ProcessPaymentAsync(creditCard))
            .ReturnsAsync(brokerResult);
        _paymentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentRepositoryMock.Setup(x => x.Update(It.IsAny<Payment>()))
            .Callback<Payment>(p => { });
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(payment);

        // Act
        var result = await _paymentService.ProcessEnrollmentPaymentAsync(enrollmentId, amount, creditCard);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Paid);
        result.TransactionId.Should().NotBeNull();
        result.PaymentDate.Should().NotBeNull();
        result.CreditCardData.Should().NotBeNull();
        result.CreditCardData.Hash.Should().Be("hash-123");
        _paymentRepositoryMock.Verify(x => x.Insert(It.Is<Payment>(p => p.EnrollmentId == enrollmentId && p.Amount == amount)), Times.Once);
        _paymentRepositoryMock.Verify(x => x.Update(It.Is<Payment>(p => p.Status == PaymentStatus.Paid)), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentAsync(enrollment), Times.Once);
        _studentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessEnrollmentPaymentAsync_ShouldHandleFailedPayment()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var amount = 99.99m;
        var creditCard = new CreditCard("1234567890123456", "Test User", "12/25", "123");
        var enrollment = new Enrollment(Guid.CreateVersion7(), Guid.CreateVersion7());
        var brokerResult = new PaymentBrokerResult(false, "Insufficient funds", null);
        var payment = new Payment(enrollmentId, amount);
        var studentUnitOfWorkMock = new Mock<IUnitOfWork>();

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _studentRepositoryMock.Setup(x => x.UnitOfWork)
            .Returns(studentUnitOfWorkMock.Object);
        studentUnitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentBrokerServiceMock.Setup(x => x.ProcessPaymentAsync(creditCard))
            .ReturnsAsync(brokerResult);
        _paymentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentRepositoryMock.Setup(x => x.Update(It.IsAny<Payment>()))
            .Callback<Payment>(p => { });
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(payment);

        // Act
        var result = await _paymentService.ProcessEnrollmentPaymentAsync(enrollmentId, amount, creditCard);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Failed);
        result.Details.Should().Be("Insufficient funds");
        _paymentRepositoryMock.Verify(x => x.Insert(It.Is<Payment>(p => p.EnrollmentId == enrollmentId && p.Amount == amount)), Times.Once);
        _paymentRepositoryMock.Verify(x => x.Update(It.Is<Payment>(p => p.Status == PaymentStatus.Failed && p.Details == "Insufficient funds")), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentAsync(enrollment), Times.Never);
    }

    [Fact]
    public async Task ProcessEnrollmentPaymentAsync_ShouldThrowWhenEnrollmentNotFound()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var amount = 99.99m;
        var creditCard = new CreditCard("1234567890123456", "Test User", "12/25", "123");

        _studentRepositoryMock.Setup(x => x.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync((Enrollment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _paymentService.ProcessEnrollmentPaymentAsync(enrollmentId, amount, creditCard));

        _paymentRepositoryMock.Verify(x => x.Insert(It.IsAny<Payment>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _studentRepositoryMock.Verify(x => x.UpdateEnrollmentAsync(It.IsAny<Enrollment>()), Times.Never);
    }

    [Fact]
    public async Task RefundPaymentAsync_ShouldRefundPaymentSuccessfully()
    {
        // Arrange
        var paymentId = Guid.CreateVersion7();
        var payment = new Payment(Guid.CreateVersion7(), 99.99m);
        payment.ProcessPayment("transaction-123");
        payment.ConfirmPayment(new CreditCardData { Hash = "hash-123" });

        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(paymentId))
            .ReturnsAsync(payment);
        _paymentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _paymentService.RefundPaymentAsync(paymentId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Refunded);
        _paymentRepositoryMock.Verify(x => x.Update(payment), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefundPaymentAsync_ShouldThrowWhenPaymentNotFound()
    {
        // Arrange
        var paymentId = Guid.CreateVersion7();
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(paymentId))
            .ReturnsAsync((Payment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _paymentService.RefundPaymentAsync(paymentId));

        _paymentRepositoryMock.Verify(x => x.Update(It.IsAny<Payment>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelPaymentAsync_ShouldCancelPaymentSuccessfully()
    {
        // Arrange
        var paymentId = Guid.CreateVersion7();
        var enrollmentId = Guid.CreateVersion7();
        var payment = new Payment(enrollmentId, 99.99m);
        // Payment starts in Pending status, which is the correct state for cancellation

        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(paymentId))
            .ReturnsAsync(payment);
        _paymentRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _paymentService.CancelPaymentAsync(paymentId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Cancelled);
        _paymentRepositoryMock.Verify(x => x.Update(payment), Times.Once);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelPaymentAsync_ShouldThrowWhenPaymentNotFound()
    {
        // Arrange
        var paymentId = Guid.CreateVersion7();
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(paymentId))
            .ReturnsAsync((Payment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _paymentService.CancelPaymentAsync(paymentId));

        _paymentRepositoryMock.Verify(x => x.Update(It.IsAny<Payment>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ShouldReturnPaymentWhenFound()
    {
        // Arrange
        var paymentId = Guid.CreateVersion7();
        var expectedPayment = new Payment(Guid.CreateVersion7(), 99.99m);
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(paymentId))
            .ReturnsAsync(expectedPayment);

        // Act
        var result = await _paymentService.GetPaymentByIdAsync(paymentId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedPayment);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ShouldReturnNullWhenPaymentNotFound()
    {
        // Arrange
        var paymentId = Guid.CreateVersion7();
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(paymentId))
            .ReturnsAsync((Payment?)null);

        // Act
        var result = await _paymentService.GetPaymentByIdAsync(paymentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPaymentsByEnrollmentIdAsync_ShouldReturnPaymentsWhenFound()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        var expectedPayments = new List<Payment>
        {
            new Payment(enrollmentId, 99.99m),
            new Payment(enrollmentId, 199.99m)
        };
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
            .ReturnsAsync(expectedPayments);

        // Act
        var result = await _paymentService.GetPaymentsByEnrollmentIdAsync(enrollmentId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedPayments);
    }

    [Fact]
    public async Task GetPaymentsByEnrollmentIdAsync_ShouldReturnEmptyListWhenNoPaymentsFound()
    {
        // Arrange
        var enrollmentId = Guid.CreateVersion7();
        _paymentRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Payment>());

        // Act
        var result = await _paymentService.GetPaymentsByEnrollmentIdAsync(enrollmentId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
} 