using Peo.Core.Interfaces.Services;
using Peo.Web.Api.Services;

namespace Peo.Web.Api.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<IAppIdentityUser, AppIdentityUser>();
            return services;
        }
    }
}