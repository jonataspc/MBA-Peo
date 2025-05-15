using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Communication.Mediator;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Core.Messages.IntegrationCommands;
using Peo.Core.Messages.IntegrationEvents;
using Peo.Faturamento.Application.Handlers;
using Peo.Faturamento.Application.Services;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Domain.Interfaces.Services;
using Peo.Faturamento.Integrations.Paypal.Services;
using Peo.GestaoAlunos.Application.Commands.Aula;
using Peo.GestaoAlunos.Application.Commands.Matricula;
using Peo.GestaoAlunos.Application.Commands.MatriculaCurso;
using Peo.GestaoAlunos.Application.Commands.PagamentoMatricula;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.EventHandlers;
using Peo.GestaoAlunos.Application.Queries.ObterCertificadosEstudante;
using Peo.GestaoAlunos.Application.Services;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoConteudo.Application.Services;
using Peo.GestaoConteudo.Application.UseCases.Aula.Cadastrar;
using Peo.Identity.Application.Services;

namespace Peo.IoC.Configuration
{
    public static class Services
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(Services).Assembly);
            });
            // Mediator
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Handlers
            services.AddScoped<INotificationHandler<PagamentoMatriculaConfirmadoEvent>, PagamentoMatriculaEventHandlers>();
            services.AddScoped<INotificationHandler<PagamentoComFalhaEvent>, PagamentoMatriculaEventHandlers>();

            // Commands
            services.AddScoped<IRequestHandler<ProcessarPagamentoMatriculaCommand, Result<ProcessarPagamentoMatriculaResponse>>, ProcessarPagamentoMatriculaCommandHandler>();

            // Conteudo
            services.AddScoped<IRequestHandler<GestaoConteudo.Application.UseCases.Curso.Cadastrar.Command, Result<GestaoConteudo.Application.UseCases.Curso.Cadastrar.Response>>, GestaoConteudo.Application.UseCases.Curso.Cadastrar.Handler>();

            services.AddScoped<IRequestHandler<GestaoConteudo.Application.UseCases.Curso.ObterPorId.Query, Result<GestaoConteudo.Application.UseCases.Curso.ObterPorId.Response>>, GestaoConteudo.Application.UseCases.Curso.ObterPorId.Handler>();

            services.AddScoped<IRequestHandler<GestaoConteudo.Application.UseCases.Curso.ObterTodos.Query, Result<GestaoConteudo.Application.UseCases.Curso.ObterTodos.Response>>, GestaoConteudo.Application.UseCases.Curso.ObterTodos.Handler>();

            services.AddScoped<IRequestHandler<GestaoConteudo.Application.UseCases.Aula.ObterTodos.Query, Result<GestaoConteudo.Application.UseCases.Aula.ObterTodos.Response>>, GestaoConteudo.Application.UseCases.Aula.ObterTodos.Handler>();

            services.AddScoped<IRequestHandler<Command, Result<GestaoConteudo.Application.UseCases.Aula.Cadastrar.Response>>, GestaoConteudo.Application.UseCases.Aula.Cadastrar.Handler>();

            // Alunos
            services.AddScoped<IRequestHandler<MatriculaCursoCommand, Result<MatriculaCursoResponse>>, MatriculaCursoCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirMatriculaCommand, Result<ConcluirMatriculaResponse>>, ConcluirMatriculaCommandHandler>();
            services.AddScoped<IRequestHandler<PagamentoMatriculaCommand, Result<PagamentoMatriculaResponse>>, PagamentoMatriculaCommandHandler>();
            services.AddScoped<IRequestHandler<IniciarAulaCommand, Result<ProgressoAulaResponse>>, IniciarAulaCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirAulaCommand, Result<ProgressoAulaResponse>>, ConcluirAulaCommandHandler>();
            services.AddScoped<IRequestHandler<ObterCertificadosEstudanteQuery, Result<IEnumerable<CertificadoEstudanteResponse>>>, ObterCertificadosEstudanteQueryHandler>();

            return services;
        }

        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IBrokerPagamentoService, PaypalBrokerService>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Application services
            services.AddScoped<IEstudanteService, EstudanteService>();
            services.AddScoped<IPagamentoService, PagamentoService>();

            // Anti-corruption layers
            services.AddScoped<ICursoAulaService, CursoAulaService>();
            services.AddScoped<IDetalhesUsuarioService, UserService>();
            return services;
        }
    }
}