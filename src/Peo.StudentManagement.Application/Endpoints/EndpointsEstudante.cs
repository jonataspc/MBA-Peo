using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;
using Peo.StudentManagement.Application.Endpoints.Aula;
using Peo.StudentManagement.Application.Endpoints.Estudante;
using Peo.StudentManagement.Application.Endpoints.Matricula;

namespace Peo.StudentManagement.Application.Endpoints
{
    public static class EndpointsEstudante
    {
        public static void MapEstudanteEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/estudante")
            .WithTags("Estudante")
            .MapEndpoint<EndpointMatriculaCurso>()
            .MapEndpoint<EndpointPagamentoMatricula>()
            .MapEndpoint<EndpointConcluirMatricula>()
            .MapEndpoint<EndpointCertificadosEstudante>()
            .MapEndpoint<EndpointsAula>()
            ;
        }
    }
}