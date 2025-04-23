using Peo.Core.Entities;
using Peo.Core.Interfaces.Data;

namespace Peo.Identity.Domain.Interfaces.Data
{
    public interface IUserRepository
    {
        IUnitOfWork UnitOfWork { get; }

        void Insert(User user);
    }
}