using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.EnrollmentPayment;

public class EnrollmentPaymentCommand : IRequest<Result<EnrollmentPaymentResponse>>
{
    public EnrollmentPaymentRequest Request { get; }

    public EnrollmentPaymentCommand(EnrollmentPaymentRequest request)
    {
        Request = request;
    }
}
