using GDemoExpress.Core;
using GDemoExpress.DataBase.Entities;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GDemoExpress.DataBase
{
    public static class MigrationDbInitializer
    {
        public static async Task<WebApplication> InitializerDatabase(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                using var appContext = scope.ServiceProvider.GetRequiredService<DboContext>();
                if (appContext.Players.AsQueryable().Any())
                {
                    return webApp;
                }
                using var transaction = appContext.Database.BeginTransaction();
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var playerId = NewId.NextGuid();
                        var nickName = string.Format("Gordon{0}", i.ToString().PadLeft(2, '0'));
                        var account = nickName.ToLower();
                        _ = appContext.Players.Add(new Player()
                        {
                            PlayerId = playerId,
                            Account = account,
                            Password = "123456",
                            Status = (int)PlayerStatus.ENABLE,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        });
                        _ = await appContext.SaveChangesAsync().ConfigureAwait(false);
                        _ = appContext.Playerinfos.Add(new Playerinfo()
                        {
                            PlayerId = playerId,
                            NickName = nickName
                        });
                        _ = await appContext.SaveChangesAsync().ConfigureAwait(false);
                    }

                    await transaction.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);

                    throw;
                }
            }
            return webApp;
        }
    }
}
