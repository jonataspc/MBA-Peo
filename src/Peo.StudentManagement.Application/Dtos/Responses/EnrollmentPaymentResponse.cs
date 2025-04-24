namespace Peo.StudentManagement.Application.Dtos.Responses;

public class EnrollmentPaymentResponse
{
    public Guid PaymentId { get; set; }
    public Guid EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? PaymentDate { get; set; }
    public string? TransactionId { get; set; }
} 