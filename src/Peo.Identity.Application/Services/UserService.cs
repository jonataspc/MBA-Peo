using Peo.Core.Entities;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Services
{
    public class UserService(IUserRepository repository) : IUserService, IUserDetailsService
    {
        public async Task AddAsync(User user)
        {
            repository.Insert(user);
            await repository.UnitOfWork.CommitAsync(CancellationToken.None);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await repository.GetByIdAsync(userId);
        }
    }
}