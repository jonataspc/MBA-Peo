using Peo.Core.Entities;

namespace Peo.Core.Interfaces.Services.Acls;

public interface IUserDetailsService
{
    Task<User?> GetUserByIdAsync(Guid userId);
}