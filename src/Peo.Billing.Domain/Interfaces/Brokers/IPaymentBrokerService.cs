using Peo.Billing.Domain.Dtos;

namespace Peo.Billing.Domain.Interfaces.Brokers
{
    public interface IPaymentBrokerService
    {
        Task<PaymentBrokerResult> ProcessPaymentAsync(CreditCard creditCard);
    }
}