namespace Peo.Faturamento.Domain.Dtos
{
    public record CartaoCredito(string? NumeroCartao, string? DataExpiracao, string? Cvv, string? Nome);
}