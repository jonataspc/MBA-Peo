using Peo.Billing.Domain.Dtos;

namespace Peo.Billing.Domain.Interfaces.Brokers
{
    public interface IBrokerPagamentoService
    {
        Task<PaymentBrokerResult> ProcessarPagamentoAsync(CartaoCredito cartaoCredito);
    }
}