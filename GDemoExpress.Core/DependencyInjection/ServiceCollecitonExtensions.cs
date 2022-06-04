using System.Reflection;
using GDemoExpress.Core;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddPlayerServerCore(
            this IServiceCollection services,
            Action<DemoExpressOptions, IServiceProvider> demoExpressConfigure)
            => services
            .AddOptions<DemoExpressOptions>()
            .Configure(demoExpressConfigure)
            .Services
            .AddMediatR(Assembly.GetExecutingAssembly());
    }
}
