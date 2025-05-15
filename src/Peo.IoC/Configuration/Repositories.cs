using Microsoft.Extensions.DependencyInjection;
using Peo.Faturamento.Domain.Entities;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.Core.Infra.Data.Repositories;
using Peo.Core.Interfaces.Data;
using Peo.Faturamento.Infra.Data.Contexts;
using Peo.GestaoAlunos.Infra.Data.Repositories;
using Peo.GestaoConteudo.Infra.Data.Repositories;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.IoC.Configuration
{
    public static class Repositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Gestao conteudo
            services.AddScoped<IRepository<Curso>, CursoRepository>();

            // Gestao alunos
            services.AddScoped<IEstudanteRepository, EstudanteRepository>();

            // Faturamento
            services.AddScoped<IRepository<Pagamento>, GenericRepository<Pagamento, CobrancaContext>>();

            return services;
        }
    }
}