using Peo.Core.Entities.Base;

namespace Peo.Identity.Domain.Entities
{
    public class User : EntityBase
    {
        public string FullName { get; private set; }
        public string Email { get; private set; }

        public User(Guid id, string fullName, string email)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Id = id;
        }
    }
}