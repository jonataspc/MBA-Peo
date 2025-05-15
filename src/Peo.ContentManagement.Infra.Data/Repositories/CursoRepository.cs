using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Infra.Data.Contexts;
using Peo.Core.Infra.Data.Repositories;

namespace Peo.ContentManagement.Infra.Data.Repositories
{
    public class CursoRepository : GenericRepository<Curso, GestaoConteudoContext>
    {
        public CursoRepository(GestaoConteudoContext dbContext) : base(dbContext)
        {
        }
    }
}