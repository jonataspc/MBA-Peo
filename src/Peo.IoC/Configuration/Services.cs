using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Billing.Application.Services;
using Peo.Billing.Domain.Interfaces.Brokers;
using Peo.Billing.Domain.Interfaces.Services;
using Peo.Billing.Integrations.Paypal.Services;
using Peo.ContentManagement.Application.Services;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Application.Services;
using Peo.StudentManagement.Application.Commands.CourseEnrollment;
using Peo.StudentManagement.Application.Commands.ProcessEnrollmentPayment;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Application.Services;
using Peo.StudentManagement.Domain.Interfaces;

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

            // Handlers:

            // Content
            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.Create.Command, Result<ContentManagement.Application.UseCases.Course.Create.Response>>, ContentManagement.Application.UseCases.Course.Create.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.GetById.Query, Result<ContentManagement.Application.UseCases.Course.GetById.Response>>, ContentManagement.Application.UseCases.Course.GetById.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Course.GetAll.Query, Result<ContentManagement.Application.UseCases.Course.GetAll.Response>>, ContentManagement.Application.UseCases.Course.GetAll.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Lesson.GetAll.Query, Result<ContentManagement.Application.UseCases.Lesson.GetAll.Response>>, ContentManagement.Application.UseCases.Lesson.GetAll.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Lesson.Create.Command, Result<ContentManagement.Application.UseCases.Lesson.Create.Response>>, ContentManagement.Application.UseCases.Lesson.Create.Handler>();

            // Students
            services.AddScoped<IRequestHandler<CourseEnrollmentCommand, Result<CourseEnrollmentResponse>>, CourseEnrollmentCommandHandler>();
            services.AddScoped<IRequestHandler<EnrollmentPaymentCommand, Result<EnrollmentPaymentResponse>>, EnrollmentPaymentCommandHandler>();

            return services;
        }

        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IPaymentBrokerService, PaypalBrokerService>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Application services
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Anti-corruption layers
            services.AddScoped<ICourseLessonService, CourseLessonService>();
            services.AddScoped<IUserDetailsService, UserService>();
            return services;
        }
    }
}