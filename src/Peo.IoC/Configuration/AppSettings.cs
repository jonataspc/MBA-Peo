using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Dtos;

namespace Peo.IoC.Configuration
{
    public static class AppSettings
    {
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Position));

            return services;
        }
    }
}