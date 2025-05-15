using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Commands.Matricula;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Endpoints.Matricula;

public class EndpointConcluirMatricula : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/matricula/concluir", Handler)
           .WithSummary("Concluir uma matrícula de curso")
           .RequireAuthorization(AccessRoles.Student);
    }

    private static async Task<Results<Ok<ConcluirMatriculaResponse>, ValidationProblem, BadRequest<Error>>> Handler(ConcluirMatriculaRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        if (!MiniValidator.TryValidate(request, out var errors))
        {
            return TypedResults.ValidationProblem(errors);
        }

        var command = new ConcluirMatriculaCommand(request);
        var response = await mediator.Send(command, cancellationToken);

        if (response.IsSuccess)
        {
            return TypedResults.Ok(response.Value);
        }

        return TypedResults.BadRequest(response.Error);
    }
}