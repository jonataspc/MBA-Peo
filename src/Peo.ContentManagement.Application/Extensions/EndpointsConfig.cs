//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Peo.Core.Web.Extensions;

//namespace Peo.ContentManagement.Application.Extensions
//{
//    public static class EndpointsConfig
//    {
//        public static WebApplication MapContentManagementEndpoints(this WebApplication app)
//        {
//            var endpoints = app
//            .MapGroup("");

//            endpoints.MapGroup("v1/content")
//            .WithTags("Content")
//            .MapEndpoint<CourseEndpoints>();

//            return app;
//        }
//    }
//}