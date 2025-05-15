using Microsoft.Extensions.DependencyInjection;
using Peo.Billing.Domain.Entities;
using Peo.Billing.Infra.Data.Contexts;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Infra.Data.Repositories;
using Peo.Core.Infra.Data.Repositories;
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
            services.AddScoped<IRepository<Curso>, CursoRepository>();

            // StudentManagement
            services.AddScoped<IEstudanteRepository, EstudanteRepository>();

            // Billing
            services.AddScoped<IRepository<Pagamento>, GenericRepository<Pagamento, BillingContext>>();

            return services;
        }
    }
}