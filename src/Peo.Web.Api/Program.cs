using Peo.IoC;
using Peo.Web.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration, builder.Environment)
                .AddSwagger()
                .SetupWebApi(builder.Configuration)
                .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseCustomSwagger(builder.Environment);
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.AddEndpoints();

await app.RunAsync();

//TODO: error handling via middleware