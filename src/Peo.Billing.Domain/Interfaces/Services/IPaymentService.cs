using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.Entities;

namespace Peo.Billing.Domain.Interfaces.Services;

public interface IPaymentService
{
    Task<Payment> RefundPaymentAsync(Guid paymentId);

    Task<Payment> CancelPaymentAsync(Guid paymentId);

    Task<Payment?> GetPaymentByIdAsync(Guid paymentId);

    Task<IEnumerable<Payment>> GetPaymentsByEnrollmentIdAsync(Guid enrollmentId);

    Task<Payment> ProcessEnrollmentPaymentAsync(Guid enrollmentId, decimal amount, CreditCard creditCard);
}