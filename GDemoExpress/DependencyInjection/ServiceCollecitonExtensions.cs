namespace GDemoExpress.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddPlayerServer(
            this IServiceCollection services,
            Action<MongoDBOptions, IServiceProvider> mongoDBconfigure,
            Action<RedisOptions, IServiceProvider> redisDBconfigure)
            => services
                .AddOptions<MongoDBOptions>()
                .Configure(mongoDBconfigure)
                .Services
                .AddOptions<RedisOptions>()
                .Configure(redisDBconfigure)
                .Services
                .AddSingleton<MongoDBConnectionManager>()
                .AddSingleton<RedisConnectionManager>();
    }
}
