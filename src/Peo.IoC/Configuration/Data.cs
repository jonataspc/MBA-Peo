using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.ContentManagement.Infra.Data.Contexts;
using Peo.Identity.Infra.Data.Contexts;

namespace Peo.IoC.Configuration
{
    public static class Data
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            string connectionString;

            if (hostEnvironment.IsDevelopment())
            {
                connectionString = configuration.GetConnectionString("SQLiteConnection") ?? throw new InvalidOperationException("Não localizada connection string para ambiente de desenvolvimento (SQLite)");
            }
            else
            {
                connectionString = configuration.GetConnectionString("SqlServerConnection") ?? throw new InvalidOperationException("Não localizada connection string para ambiente de produção (SQL Server)");
            }

            // Identity
            services.AddDbContext<IdentityContext>(options =>
            {
                if (hostEnvironment.IsDevelopment())
                {
                    options.UseSqlite(connectionString);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }

                options.UseLazyLoadingProxies();

                if (hostEnvironment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            });

            // BCs:
            // ContentManagement
            services.AddDbContext<ContentManagementContext>(options =>
            {
                if (hostEnvironment.IsDevelopment())
                {
                    options.UseSqlite(connectionString);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }

                options.UseLazyLoadingProxies();

                if (hostEnvironment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            });

            if (hostEnvironment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            return services;
        }
    }
}