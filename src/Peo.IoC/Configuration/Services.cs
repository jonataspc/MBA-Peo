using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Billing.Domain.Interfaces.Brokers;
using Peo.Billing.Integrations.Paypal.Services;
using Peo.ContentManagement.Application.Services;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Application.Services;

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

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.GetById.Query, Result<ContentManagement.Application.UseCases.Course.GetById.Response>>, ContentManagement.Application.UseCases.Course.GetById.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.GetAll.Query, Result<ContentManagement.Application.UseCases.Course.GetAll.Response>>, ContentManagement.Application.UseCases.Course.GetAll.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Lesson.GetAll.Query, Result<ContentManagement.Application.UseCases.Lesson.GetAll.Response>>, ContentManagement.Application.UseCases.Lesson.GetAll.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Lesson.Create.Command, Result<ContentManagement.Application.UseCases.Lesson.Create.Response>>, ContentManagement.Application.UseCases.Lesson.Create.Handler>();

            return services;
        }

        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IPaymentBrokerService, PaypalBrokerService>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Anti-corruption layers
            services.AddScoped<ICourseLessonService, CourseLessonService>();
            services.AddScoped<IUserDetailsService, UserService>();
            return services;
        }
    }
}