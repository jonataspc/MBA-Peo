using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.Interfaces.Brokers;

namespace Peo.Billing.Integrations.Paypal.Services
{
    public class PaypalBrokerService : IPaymentBrokerService
    {
        public async Task<PaymentBrokerResult> ProcessPaymentAsync(CreditCard creditCard)
        {
            // simulates an API call to Paypal ...

            await Task.Delay(TimeSpan.FromSeconds(5));

            return new PaymentBrokerResult(true, default, Guid.NewGuid().ToString());
        }
    }
}