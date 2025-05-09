﻿using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Peo.ContentManagement.Application.Dtos;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;

namespace Peo.ContentManagement.Application.UseCases.Course.GetById
{
    public class Endpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/course/{id:guid}", HandleGetById)
             .WithSummary("Get course by ID")
             .RequireAuthorization(AccessRoles.Admin);
        }

        private static async Task<Results<Ok<CourseResponse>, NotFound, ValidationProblem, BadRequest, BadRequest<Error>>> HandleGetById(Guid id, IMediator mediator, ILogger<Endpoint> logger)
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

            if (result.Value.Course is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value.Course);
        }
    }
}