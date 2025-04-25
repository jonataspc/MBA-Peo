using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Application.Queries.GetStudentCertificates;

namespace Peo.StudentManagement.Application.Endpoints.Student;

public class GetStudentCertificatesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/certificates", Handler)
           .WithSummary("Get student certificates")
           .RequireAuthorization(AccessRoles.Student);
    }

    private static async Task<Results<Ok<IEnumerable<StudentCertificateResponse>>, BadRequest<Error>>> Handler(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetStudentCertificatesQuery();
        var response = await mediator.Send(query, cancellationToken);

        if (response.IsSuccess)
        {
            return TypedResults.Ok(response.Value);
        }

        return TypedResults.BadRequest(response.Error);
    }
}