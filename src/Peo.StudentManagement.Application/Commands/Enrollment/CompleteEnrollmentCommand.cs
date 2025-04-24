using MediatR;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.Enrollment;

public class CompleteEnrollmentCommand : IRequest<Result<CompleteEnrollmentResponse>>
{
    public CompleteEnrollmentRequest Request { get; }

    public CompleteEnrollmentCommand(CompleteEnrollmentRequest request)
    {
        Request = request;
    }
} 