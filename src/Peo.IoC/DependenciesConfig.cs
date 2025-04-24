using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.IoC.Configuration;

namespace Peo.IoC
{
    public static class DependenciesConfig
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDataDependencies(configuration, hostEnvironment)
                    .AddIdentity()
                    .AddAppSettings(configuration)
                    .AddRepositories()
                    .AddServices()
                    .AddMediator()
                    .AddExternalServices();

            return services;
        }
    }
}