﻿using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;
using Peo.GestaoConteudo.Application.Dtos;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterPorId
{
    public class Endpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/curso/{id:guid}", HandleGetById)
             .WithSummary("Obter curso por Id")
             .RequireAuthorization(AccessRoles.Admin);
        }

        private static async Task<Results<Ok<CursoResponse>, NotFound, ValidationProblem, BadRequest, BadRequest<Error>>> HandleGetById(Guid id, IMediator mediator, ILogger<Endpoint> logger)
        {
            var query = new Query(id);

            Result<Response> result;

            try
            {
                result = await mediator.Send(query);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return TypedResults.BadRequest();
            }

            if (!result.IsSuccess)
            {
                return TypedResults.BadRequest(result.Error);
            }

            if (result.Value.Curso is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value.Curso);
        }
    }
}