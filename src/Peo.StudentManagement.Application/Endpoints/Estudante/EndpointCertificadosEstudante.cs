using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Application.Queries.GetStudentCertificates;

namespace Peo.StudentManagement.Application.Endpoints.Estudante;

public class EndpointCertificadosEstudante : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/certificados", Handler)
           .WithSummary("Obter certificados do estudante")
           .RequireAuthorization(AccessRoles.Student);
    }

    private static async Task<Results<Ok<IEnumerable<CertificadoEstudanteResponse>>, BadRequest<Error>>> Handler(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new ObterCertificadosEstudanteQuery();
        var response = await mediator.Send(query, cancellationToken);

        if (response.IsSuccess)
        {
            return TypedResults.Ok(response.Value);
        }

        return TypedResults.BadRequest(response.Error);
    }
}