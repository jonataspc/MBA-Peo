using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MiniValidation;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Endpoints
{
    internal class Enrollment : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/enrollment/", Handle)
             .WithSummary("Enroll in a new course")
             .RequireAuthorization(AccessRoles.Student);
        }

        private static async Task<Results<Ok<EnrollmentResponse>, BadRequest<string>, ValidationProblem>> Handle(
            EnrollmentRequest request,
            IStudentService studentService,
            IAppIdentityUser appIdentityUser,
            ILogger<Enrollment> logger,
            CancellationToken cancellationToken)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            Domain.Entities.Enrollment enrollment;

            try
            {
                enrollment = await studentService.EnrollStudentWithUserIdAsync(appIdentityUser.GetUserId(), request.CourseId, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return TypedResults.BadRequest(e.Message);
            }

            return TypedResults.Ok(new EnrollmentResponse(enrollment.Id));
        }
    }
}