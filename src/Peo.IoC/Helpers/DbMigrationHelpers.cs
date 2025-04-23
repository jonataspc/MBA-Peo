using Microsoft.AspNetCore.Builder;
using Peo.ContentManagement.Infra.Data.Helpers;
using Peo.Identity.Infra.Data.Helpers;

namespace Peo.IoC.Helpers
{
    public static class DbMigrationHelpers
    {
        public static async Task UseDbMigrationHelperAsync(this WebApplication app)
        {
            await app.UseIdentityDbMigrationHelperAsync();
            await app.UseContentManagementDbMigrationHelperAsync();
        }
    }
}