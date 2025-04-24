using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Commands.CourseEnrollment;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Endpoints.Enrollment
{
    public class CourseEnrollmentEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/enrollment/", Handle)
              .WithSummary("Enroll in a new course")
              .RequireAuthorization(AccessRoles.Student);
        }

        private static async Task<Results<Ok<CourseEnrollmentResponse>, ValidationProblem, BadRequest<Error>>> Handle(CourseEnrollmentRequest request, IMediator mediator, CancellationToken cancellationToken)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var command = new CourseEnrollmentCommand(request);
            var response = await mediator.Send(command, cancellationToken);

            if (response.IsSuccess)
            {
                return TypedResults.Ok(response.Value);
            }

            return TypedResults.BadRequest(response.Error);

        }
    }

    //public class CourseEnrollmentEndpoint : IEndpoint
    //{
    //    public static void Map(IEndpointRouteBuilder app)
    //    {
    //        app.MapPost("/enrollment/", Handle)
    //         .WithSummary("Enroll in a new course")
    //         .RequireAuthorization(AccessRoles.Student);
    //    }

    //private static async Task<Results<Ok<CourseEnrollmentResponse>, BadRequest<string>, ValidationProblem>> Handle(
    //    CourseEnrollmentRequest request,
    //    IStudentService studentService,
    //    IAppIdentityUser appIdentityUser,
    //    ILogger<CourseEnrollmentEndpoint> logger,
    //    CancellationToken cancellationToken)
    //{
    //    if (!MiniValidator.TryValidate(request, out var errors))
    //    {
    //        return TypedResults.ValidationProblem(errors);
    //    }

    //    Domain.Entities.Enrollment enrollment;

    //    try
    //    {
    //        enrollment = await studentService.EnrollStudentWithUserIdAsync(appIdentityUser.GetUserId(), request.CourseId, cancellationToken);
    //    }
    //    catch (Exception e)
    //    {
    //        logger.LogError(e, e.Message);
    //        return TypedResults.BadRequest(e.Message);
    //    }

    //    return TypedResults.Ok(new CourseEnrollmentResponse(enrollment.Id));
    //}
}
