using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;

namespace Peo.StudentManagement.Application.Endpoints
{
    public static class StudentEndpoints
    {
        public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/student")
            .WithTags("Student")
            .MapEndpoint<Enrollment>();
        }
    }
}