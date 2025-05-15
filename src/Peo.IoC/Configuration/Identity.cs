﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Peo.GestaoAlunos.Application.Endpoints;
using Peo.GestaoConteudo.Application;
using Peo.Identity.Application.Extensions;
using Peo.Identity.Application.Services;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;
using Peo.Identity.Infra.Data.Contexts;
using Peo.Identity.Infra.Data.Repositories;

namespace Peo.IoC.Configuration
{
    internal static class Identity
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                     .AddRoles<IdentityRole>()
                     .AddEntityFrameworkStores<IdentityContext>()
                     .AddApiEndpoints();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapIdentityEndpoints();
            app.MapCursoEndpoints();
            app.MapEstudanteEndpoints();
            return app;
        }
    }
}