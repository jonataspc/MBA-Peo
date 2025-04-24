using Microsoft.Extensions.DependencyInjection;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Infra.Data.Repositories;
using Peo.Core.Interfaces.Data;
using Peo.StudentManagement.Domain.Interfaces;
using Peo.StudentManagement.Infra.Data.Repositories;

namespace Peo.IoC.Configuration
{
    public static class Repositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // ContentManagement
            services.AddScoped<IRepository<Course>, CourseRepository>();

            // StudentManagement
            services.AddScoped<IStudentRepository, StudentRepository>();
            return services;
        }
    }
}