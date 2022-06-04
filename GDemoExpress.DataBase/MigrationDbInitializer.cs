using GDemoExpress.Core;
using GDemoExpress.DataBase.Entities;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.Encrypt.Extensions;

namespace GDemoExpress.DataBase
{
    public static class MigrationDbInitializer
    {
        public static async Task<WebApplication> InitializerDatabase(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var configuration = sp.GetRequiredService<IConfiguration>();
                var demoExpress = configuration.GetSection("DemoExpress").Get<DemoExpressOptions>();
                var sysDateTime = DateTime.UtcNow;
                using var appContext = sp.GetRequiredService<DboContext>();
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
                        var nickName = string.Format("Gordon{0}", (i + 1).ToString().PadLeft(2, '0'));
                        var account = nickName.ToLower();
                        var hashed = demoExpress.PlayerPassword.HMACSHA512(demoExpress.PlayerCryptographyKey);
                        _ = appContext.Players.Add(new Player()
                        {
                            PlayerId = playerId,
                            Account = account,
                            Password = hashed,
                            Status = (int)PlayerStatus.ENABLE,
                            CreatedOn = sysDateTime,
                            UpdatedOn = sysDateTime
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
