using Peo.Identity.Domain.Entities;

namespace Peo.Identity.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task AddAsync(User user);
    }
}