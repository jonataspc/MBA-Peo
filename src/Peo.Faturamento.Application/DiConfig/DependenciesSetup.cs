﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Dtos;
using Peo.Core.Messages.IntegrationCommands;
using Peo.Faturamento.Application.Handlers;
using Peo.Faturamento.Application.Services;
using Peo.Faturamento.Domain.Interfaces.Services;

namespace Peo.Faturamento.Application.DiConfig
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForFaturamento(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(PagamentoService).Assembly);
            });

            // Commands
            services.AddScoped<IRequestHandler<ProcessarPagamentoMatriculaCommand, Result<ProcessarPagamentoMatriculaResponse>>, ProcessarPagamentoMatriculaCommandHandler>();

            // Application services            
            services.AddScoped<IPagamentoService, PagamentoService>();

            return services;
        }
    }
}