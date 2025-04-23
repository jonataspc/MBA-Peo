using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MiniValidation;
using Peo.ContentManagement.Application.UseCases.Course.Create;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;

namespace Peo.ContentManagement.Application
{
    public class CourseEndpoints : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/course/", HandleCreate)
               .WithSummary("Create a new course")
               .RequireAuthorization(AccessRoles.Admin);

            app.MapGet("/course/{titleWildcard}", HandleGetAll)
               .WithSummary("Get all courses")
               .RequireAuthorization(AccessRoles.Admin);

        }

        private static async Task<Results<Ok<UseCases.Course.GetAll.Response>, ValidationProblem, BadRequest, BadRequest<Error>>> HandleGetAll(string titleWildcard, IMediator mediator, ILogger<CourseEndpoints> logger)
        {
            var command = new UseCases.Course.GetAll.Query(titleWildcard);

            Result<UseCases.Course.GetAll.Response> result;

            try
            {
                result = await mediator.Send(command);
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

            return TypedResults.Ok(result.Value);
        }

        private static async Task<Results<Ok<Response>, ValidationProblem, BadRequest, BadRequest<Error>>> HandleCreate(UseCases.Course.Create.Command command, IMediator mediator, ILogger<CourseEndpoints> logger)
        {
            if (!MiniValidator.TryValidate(command, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            Result<Response> result;

            try
            {
                result = await mediator.Send(command);
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

            return TypedResults.Ok(result.Value);
        }
    }
}