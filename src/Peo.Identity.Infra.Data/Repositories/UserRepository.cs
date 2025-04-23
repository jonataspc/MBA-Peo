using Peo.Core.Entities;
using Peo.Core.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Infra.Data.Contexts;

namespace Peo.Identity.Infra.Data.Repositories
{
    public class UserRepository(IdentityContext context) : IUserRepository
    {
        public IUnitOfWork UnitOfWork => context;

        public void Insert(User user)
        {
            context.ApplicationUsers.Add(user);
        }
    }
}