using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.Entities;
using Peo.Billing.Domain.Interfaces.Brokers;
using Peo.Billing.Domain.Interfaces.Services;
using Peo.Billing.Domain.ValueObjects;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Data;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.Billing.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPaymentBrokerService _paymentBrokerService;

    public PaymentService(IRepository<Payment> paymentRepository, IStudentRepository studentRepository, IPaymentBrokerService paymentBrokerService)
    {
        _paymentRepository = paymentRepository;
        _studentRepository = studentRepository;
        _paymentBrokerService = paymentBrokerService;
    }

    private async Task<Payment> CreatePaymentAsync(Guid enrollmentId, decimal amount)
    {
        var payment = new Payment(enrollmentId, amount);
        _paymentRepository.Insert(payment);
        await _paymentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return payment;
    }

    private async Task<Payment> ProcessPaymentAsync(Guid paymentId, string transactionId)
    {
        var payment = await GetPaymentByIdAsync(paymentId)
            ?? throw new InvalidOperationException($"Payment with ID {paymentId} not found");

        payment.ProcessPayment(transactionId);
        _paymentRepository.Update(payment);
        await _paymentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return payment;
    }
     

    public async Task<Payment> RefundPaymentAsync(Guid paymentId)
    {
        var payment = await GetPaymentByIdAsync(paymentId)
            ?? throw new InvalidOperationException($"Payment with ID {paymentId} not found");

        payment.Refund();
        _paymentRepository.Update(payment);
        await _paymentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return payment;
    }

    public async Task<Payment> CancelPaymentAsync(Guid paymentId)
    {
        var payment = await GetPaymentByIdAsync(paymentId)
            ?? throw new InvalidOperationException($"Payment with ID {paymentId} not found");

        payment.Cancel();
        _paymentRepository.Update(payment);
        await _paymentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return payment;
    }

    public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
    {
        return await _paymentRepository.WithTracking()
                                       .GetAsync(paymentId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByEnrollmentIdAsync(Guid enrollmentId)
    {
        return await _paymentRepository.WithTracking().GetAsync(p => p.EnrollmentId == enrollmentId)
            ?? Enumerable.Empty<Payment>();
    }

    public async Task<Payment> ProcessEnrollmentPaymentAsync(Guid enrollmentId, decimal amount, CreditCard creditCard)
    {
        // Get the enrollment
        var enrollment = await _studentRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new InvalidOperationException($"Enrollment with ID {enrollmentId} not found");

        // Create and process the payment
        var payment = await CreatePaymentAsync(enrollmentId, amount);
        var transactionid = Guid.NewGuid().ToString();
        payment = await ProcessPaymentAsync(payment.Id, transactionid);

        // call external broker service
        PaymentBrokerResult result;
        try
        {
            result = await _paymentBrokerService.ProcessPaymentAsync(creditCard);
        }
        catch (Exception e)
        {
            payment.MarkAsFailed(e.Message);
            throw new DomainException(e.Message);
        }

        if (result.Success)
        {
            payment.ConfirmPayment(new CreditCardData() { Hash = result.Hash });
        }
        else
        {
            payment.MarkAsFailed(result.Details);
        }

        // If payment is successful, update enrollment status
        if (payment.Status == PaymentStatus.Paid)
        {
            enrollment.PaymentDone();
            await _studentRepository.UpdateEnrollmentAsync(enrollment);
        }

        await _studentRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        await _paymentRepository.UnitOfWork.CommitAsync(CancellationToken.None);

        return payment;
    }
}