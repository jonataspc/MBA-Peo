using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Commands.Aula;
using Peo.StudentManagement.Application.Dtos.Requests;

namespace Peo.StudentManagement.Application.Endpoints.Aula;

public class EndpointsAula : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/matricula/aula/iniciar", async (
            IniciarAulaRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var command = new IniciarAulaCommand(request);
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
           .WithSummary("Iniciar uma aula")
           .RequireAuthorization(AccessRoles.Student);

        app.MapPost("/matricula/aula/concluir", async (
            ConcluirAulaRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                var command = new ConcluirAulaCommand(request);
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
           .WithSummary("Concluir uma aula")
           .RequireAuthorization(AccessRoles.Student);
    }
}