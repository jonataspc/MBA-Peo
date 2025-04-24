using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;
using Peo.StudentManagement.Application.Endpoints.Enrollment;
using Peo.StudentManagement.Application.Endpoints.Lesson;

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
            .MapEndpoint<CourseEnrollmentEndpoint>()
            .MapEndpoint<EnrollmentPaymentEndpoint>()
            .MapEndpoint<CompleteEnrollmentEndpoint>()
            .MapEndpoint<LessonEndpoints>()
            ;
        }
    }
}