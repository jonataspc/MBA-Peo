using Peo.Billing.Domain.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests;

public record EnrollmentPaymentRequest(
    [Required]
    Guid EnrollmentId,

    [Required]
    CreditCard CreditCard);