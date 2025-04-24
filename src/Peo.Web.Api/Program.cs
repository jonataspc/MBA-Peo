using Peo.IoC;
using Peo.IoC.Helpers;
using Peo.Web.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration, builder.Environment)
                .AddApiServices()
                .AddSwagger()
                .SetupWebApi(builder.Configuration)
                .AddPolicies()
                .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseCustomSwagger(builder.Environment);
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.AddEndpoints();

await app.UseDbMigrationHelperAsync();
await app.RunAsync();

//TODO: error handling via middleware