using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.DomainObjects.Result;

namespace Peo.IoC.Configuration
{
    public static class Services
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(Services).Assembly);
            });

            // Handlers
            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.Create.Command, Result<ContentManagement.Application.UseCases.Course.Create.Response>>, ContentManagement.Application.UseCases.Course.Create.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.GetAll.Query, Result<ContentManagement.Application.UseCases.Course.GetAll.Response>>, ContentManagement.Application.UseCases.Course.GetAll.Handler>();

            return services;
        }
    }
}