using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Infra.Data.Contexts;
using Peo.Core.Infra.Data.Repositories;

namespace Peo.ContentManagement.Infra.Data.Repositories
{
    public class CourseRepository : GenericRepository<Course, ContentManagementContext>
    {
        public CourseRepository(ContentManagementContext dbContext) : base(dbContext)
        {
        }
    }
}