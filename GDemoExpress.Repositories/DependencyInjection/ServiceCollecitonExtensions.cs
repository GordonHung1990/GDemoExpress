using GDemoExpress.Core;
using GDemoExpress.DataBase.Entities;
using GDemoExpress.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddPlayerServerRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> efConfigure,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver,
            Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
            => services
                .AddDbContext<DboContext>(efConfigure)
                .AddTransient<IPlayer, PlayerRepository>()
                .Decorate<IPlayer>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerRedisRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromHours(1)))
                .Decorate<IPlayer>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerRecordRepository>(sp, innerRepository, mongoDatabaseResolver(sp)));
    }
}
