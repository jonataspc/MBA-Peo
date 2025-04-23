using Microsoft.Extensions.DependencyInjection;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Infra.Data.Repositories;
using Peo.Core.Interfaces.Data;

namespace Peo.IoC.Configuration
{
    public static class Repositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // ContentManagement
            services.AddScoped<IRepository<Course>, CourseRepository>();
            return services;
        }
    }
}