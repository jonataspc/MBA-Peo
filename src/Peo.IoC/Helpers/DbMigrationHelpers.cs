using Microsoft.AspNetCore.Builder;
using Peo.Identity.Infra.Data.Helpers;

namespace Peo.IoC.Helpers
{
    public static class DbMigrationHelpers
    {
        public static async Task UseDbMigrationHelperAsync(this WebApplication app)
        {
            // Identity
            await app.UseIdentityDbMigrationHelperAsync();
        }
    }
}