using Peo.Billing.Domain.ValueObjects;

namespace Peo.StudentManagement.Application.Dtos.Responses;

public class EnrollmentPaymentResponse
{
    public Guid PaymentId { get; set; }
    public Guid EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? TransactionId { get; set; }
} 