using System.Text.Json.Serialization;

namespace Peo.Web.Api.Configuration
{
    public static class ApiConfiguration
    {
        public static IServiceCollection SetupWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetRequiredSection("Security:CorsPolicy:AllowedOrigins").Get<List<string>>()!;

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(allowedOrigins.ToArray())
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Adiciona serviços da API
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            return services;
        }
    }
}
