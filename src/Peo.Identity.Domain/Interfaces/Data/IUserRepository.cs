using Peo.Core.Entities;
using Peo.Core.Interfaces.Data;

namespace Peo.Identity.Domain.Interfaces.Data
{
    public interface IUserRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task<User?> GetByIdAsync(Guid userId);

        void Insert(User user);
    }
}