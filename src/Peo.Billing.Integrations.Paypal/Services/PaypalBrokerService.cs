using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.Interfaces.Brokers;

namespace Peo.Billing.Integrations.Paypal.Services
{
    public class PaypalBrokerService : IPaymentBrokerService
    {
        public async Task<PaymentBrokerResult> ProcessPaymentAsync(CreditCard creditCard)
        {
            if (creditCard?.CardNumber is null) {
                return new PaymentBrokerResult(false, "Credit card is null", Guid.NewGuid().ToString());
            }

            if (creditCard.CardNumber.Length != 16 && creditCard.CardNumber.Length != 15)
            {
                return new PaymentBrokerResult(false, "Credit card is invalid", Guid.NewGuid().ToString());
            }

            // simulates an API call to Paypal ...
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(0, 2)));
            
            return new PaymentBrokerResult(true, default, Guid.NewGuid().ToString());
        }
    }
}