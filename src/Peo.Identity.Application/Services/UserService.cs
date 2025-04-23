using Peo.Identity.Domain.Entities;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Services
{
    public class UserService(IUserRepository repository) : IUserService
    {
        public async Task AddAsync(User user)
        {
            repository.Insert(user);
            await repository.UnitOfWork.CommitAsync();
        }
    }
}