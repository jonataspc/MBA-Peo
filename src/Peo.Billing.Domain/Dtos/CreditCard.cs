namespace Peo.Billing.Domain.Dtos
{
    public record CreditCard(string? CardNumber, string? ExpirationDate, string? Cvv, string? Name);
}