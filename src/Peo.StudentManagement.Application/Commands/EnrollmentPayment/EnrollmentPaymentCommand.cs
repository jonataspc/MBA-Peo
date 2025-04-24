using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.ProcessEnrollmentPayment;

public class EnrollmentPaymentCommand : IRequest<Result<EnrollmentPaymentResponse>>
{
    public EnrollmentPaymentRequest Request { get; }

    public EnrollmentPaymentCommand(EnrollmentPaymentRequest request)
    {
        Request = request;
    }
}
