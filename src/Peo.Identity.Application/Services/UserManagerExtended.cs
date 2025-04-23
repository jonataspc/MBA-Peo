using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peo.Identity.Domain.Entities;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Services
{
    public class UserManagerExtended : UserManager<IdentityUser>
    {
        private readonly IUserService _userService;

        public UserManagerExtended(IUserStore<IdentityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators, IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManagerExtended> logger, IUserService userService) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userService = userService;
        }

        public override async Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            var result = await base.CreateAsync(user, password);

            await _userService.AddAsync(
                new User(Guid.Parse(user.Id), user.UserName!, user.Email!));

            return result;
        }
    }
}