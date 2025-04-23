using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using Peo.Core.Web.Api;
using Peo.Identity.Application.Endpoints.Requests;

namespace Peo.Identity.Application.Endpoints
{
    public class RegisterEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/register", HandleRegister)
               .WithSummary("Cadastra novo usuário")
               .AllowAnonymous();
        }

        private static async Task<IResult> HandleRegister(RegisterRequest request, UserManager<IdentityUser> userManager)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var user = new IdentityUser
            {
                UserName = request.Name,
                Email = request.Email,
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));
                return TypedResults.NoContent();
            }

            return Results.BadRequest(new { Description = "Errors", Content = result.Errors });
        }
    }
}