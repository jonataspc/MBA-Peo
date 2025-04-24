using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Commands.Lesson;
using Peo.StudentManagement.Application.Dtos.Requests;

namespace Peo.StudentManagement.Application.Endpoints.Lesson;

public class LessonEndpoints : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/enrollment/lesson/start", async (
            StartLessonRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var command = new StartLessonCommand(request);
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
           .WithSummary("Start a lesson")
           .RequireAuthorization(AccessRoles.Student);

        app.MapPost("/enrollment/lesson/end", async (
            EndLessonRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var command = new EndLessonCommand(request);
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
           .WithSummary("End a lesson")
           .RequireAuthorization(AccessRoles.Student);
    }
}