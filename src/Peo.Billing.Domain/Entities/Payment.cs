using Peo.Billing.Domain.ValueObjects;
using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;

namespace Peo.Billing.Domain.Entities;

public class Payment : EntityBase, IAggregateRoot
{
    public Guid EnrollmentId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? Details { get; private set; }
    public CreditCardData? CreditCardData { get; private set; }

    protected Payment()
    { }

    public Payment(Guid enrollmentId, decimal amount)
    {
        EnrollmentId = enrollmentId;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    public void ProcessPayment(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment can only be processed when in Pending status");

        TransactionId = transactionId;
        Status = PaymentStatus.Processing;
    }

    public void ConfirmPayment(CreditCardData creditCardData)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Payment can only be confirmed when in Processing status");

        CreditCardData = creditCardData;
        PaymentDate = DateTime.Now;
        Status = PaymentStatus.Paid;
    }

    public void MarkAsFailed(string? details)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Payment can only be marked as failed when in Processing status");

        Details = details;
        Status = PaymentStatus.Failed;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Paid)
            throw new InvalidOperationException("Payment can only be refunded when in Paid status");

        Status = PaymentStatus.Refunded;
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment can only be cancelled when in Pending status");

        Status = PaymentStatus.Cancelled;
    }
}