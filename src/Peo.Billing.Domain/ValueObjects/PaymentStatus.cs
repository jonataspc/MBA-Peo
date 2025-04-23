namespace Peo.Billing.Domain.ValueObjects;

public enum PaymentStatus
{
    Pending,
    Processing,
    Paid,
    Failed,
    Refunded,
    Cancelled
}
