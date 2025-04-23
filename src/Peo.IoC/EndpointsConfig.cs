using Microsoft.AspNetCore.Builder;
using Peo.IoC.Configuration;

namespace Peo.IoC
{
    public static class EndpointsConfig
    {
    public static WebApplication AddEndpoints(this WebApplication app)
        {
            app.MapEndpoints();
            return app;
        }
    
    }
}