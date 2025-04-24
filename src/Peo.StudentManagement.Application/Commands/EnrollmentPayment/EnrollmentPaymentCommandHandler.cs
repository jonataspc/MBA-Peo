using MediatR;
using Peo.Billing.Domain.Interfaces.Services;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Services.Acls;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.StudentManagement.Application.Commands.EnrollmentPayment;

public class EnrollmentPaymentCommandHandler : IRequestHandler<EnrollmentPaymentCommand, Result<EnrollmentPaymentResponse>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IPaymentService _paymentService;
    private readonly ICourseLessonService _courseLessonService;

    public EnrollmentPaymentCommandHandler(
        IStudentRepository studentRepository,
        IPaymentService paymentService,
        ICourseLessonService courseLessonService)
    {
        _studentRepository = studentRepository;
        _paymentService = paymentService;
        _courseLessonService = courseLessonService;
    }

    public async Task<Result<EnrollmentPaymentResponse>> Handle(EnrollmentPaymentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _studentRepository.GetEnrollmentByIdAsync(request.Request.EnrollmentId);

        if (enrollment is null)
        {
            return Result.Failure<EnrollmentPaymentResponse>(new Error("Enrollment not found"));
        }

        if (enrollment.Status != EnrollmentStatus.PendingPayment)
        {
            return Result.Failure<EnrollmentPaymentResponse>(new Error($"Cannot process payment for enrollment in {enrollment.Status} status"));
        }

        var coursePrice = await _courseLessonService.GetCoursePriceAsync(enrollment.CourseId);

        var payment = await _paymentService.ProcessEnrollmentPaymentAsync(
            enrollment.Id,
            coursePrice,
            request.Request.CreditCard
        );

        var response = new EnrollmentPaymentResponse
        {
            PaymentId = payment.Id,
            EnrollmentId = payment.EnrollmentId,
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentDate = payment.PaymentDate,
            TransactionId = payment.TransactionId
        };

        if (payment.Status == Billing.Domain.ValueObjects.PaymentStatus.Failed)
        {
            return Result.Failure<EnrollmentPaymentResponse>(new Error($"Payment failed: {payment.Details}"));
        }

        return Result.Success(response);
    }
}