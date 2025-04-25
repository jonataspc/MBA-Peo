using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Commands.CourseEnrollment;

public class CourseEnrollmentCommandHandler(IStudentService studentService, IAppIdentityUser appIdentityUser, ILogger<CourseEnrollmentCommandHandler> logger) : IRequestHandler<CourseEnrollmentCommand, Result<CourseEnrollmentResponse>>
{
    public async Task<Result<CourseEnrollmentResponse>> Handle(CourseEnrollmentCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Enrollment enrollment;

        try
        {
            enrollment = await studentService.EnrollStudentWithUserIdAsync(appIdentityUser.GetUserId(), request.Request.CourseId, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return Result.Failure<CourseEnrollmentResponse>(new Error(e.Message));
        }

        return Result.Success(new CourseEnrollmentResponse(enrollment.Id));
    }
}