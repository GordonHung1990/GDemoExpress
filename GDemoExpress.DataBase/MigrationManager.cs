using GDemoExpress.DataBase.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GDemoExpress.DataBase
{
    public static class MigrationManager
    {
        public static WebApplication MigrateDatabase(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                using var appContext = scope.ServiceProvider.GetRequiredService<DboContext>();
                try
                {
                    appContext.Database.Migrate();
                }
                catch (Exception)
                {
                    //Log errors or do anything you think it's needed
                    throw;
                }
            }
            return webApp;
        }
    }
}
