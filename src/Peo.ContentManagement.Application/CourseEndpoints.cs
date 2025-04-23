using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;

namespace Peo.ContentManagement.Application
{
    public static class CourseEndpoints
    {
        public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/content")
            .WithTags("Content")
            .MapEndpoint<UseCases.Course.Create.Endpoint>()
            .MapEndpoint<UseCases.Course.GetById.Endpoint>()
            .MapEndpoint<UseCases.Course.GetAll.Endpoint>()
            .MapEndpoint<UseCases.Lesson.GetAll.Endpoint>()
            .MapEndpoint<UseCases.Lesson.Create.Endpoint>();
        }
    }
}