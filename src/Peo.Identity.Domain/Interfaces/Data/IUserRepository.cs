using Peo.Core.Interfaces.Data;
using Peo.Identity.Domain.Entities;

namespace Peo.Identity.Domain.Interfaces.Data
{
    public interface IUserRepository
    {
        IUnitOfWork UnitOfWork { get; }

        void Insert(User user);
    }
}