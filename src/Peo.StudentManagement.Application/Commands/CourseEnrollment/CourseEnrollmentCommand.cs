using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.CourseEnrollment;

public class CourseEnrollmentCommand : IRequest<Result<CourseEnrollmentResponse>>
{
    public CourseEnrollmentRequest Request { get; }

    public CourseEnrollmentCommand(CourseEnrollmentRequest request)
    {
        Request = request;
    }
}