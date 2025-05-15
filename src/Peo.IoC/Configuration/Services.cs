using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Billing.Application.Services;
using Peo.Billing.Domain.Interfaces.Brokers;
using Peo.Billing.Domain.Interfaces.Services;
using Peo.Billing.Integrations.Paypal.Services;
using Peo.ContentManagement.Application.Services;
using Peo.ContentManagement.Application.UseCases.Aula.Cadastrar;
using Peo.ContentManagement.Application.UseCases.Aula.ObterTodos;
using Peo.ContentManagement.Application.UseCases.Curso.Cadastrar;
using Peo.ContentManagement.Application.UseCases.Curso.ObterPorId;
using Peo.ContentManagement.Application.UseCases.Curso.ObterTodos;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Application.Services;
using Peo.StudentManagement.Application.Commands.Aula;
using Peo.StudentManagement.Application.Commands.Matricula;
using Peo.StudentManagement.Application.Commands.MatriculaCurso;
using Peo.StudentManagement.Application.Commands.PagamentoMatricula;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Application.Queries.GetStudentCertificates;
using Peo.StudentManagement.Application.Services;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.IoC.Configuration
{
    public static class Services
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(Services).Assembly);
            });

            // Handlers:

            // Content
            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Curso.Cadastrar.Command, Result<ContentManagement.Application.UseCases.Curso.Cadastrar.Response>>, ContentManagement.Application.UseCases.Curso.Cadastrar.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Curso.ObterPorId.Query, Result<ContentManagement.Application.UseCases.Curso.ObterPorId.Response>>, ContentManagement.Application.UseCases.Curso.ObterPorId.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Curso.ObterTodos.Query, Result<ContentManagement.Application.UseCases.Curso.ObterTodos.Response>>, ContentManagement.Application.UseCases.Curso.ObterTodos.Handler>();


            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Aula.ObterTodos.Query, Result<ContentManagement.Application.UseCases.Aula.ObterTodos.Response>>, ContentManagement.Application.UseCases.Aula.ObterTodos.Handler>();

            services.AddScoped<IRequestHandler<ContentManagement.Application.UseCases.Aula.Cadastrar.Command, Result<ContentManagement.Application.UseCases.Aula.Cadastrar.Response>>, ContentManagement.Application.UseCases.Aula.Cadastrar.Handler>();



            // Students
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