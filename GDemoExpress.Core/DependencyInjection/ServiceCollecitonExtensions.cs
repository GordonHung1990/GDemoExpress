using System.Reflection;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddPlayerServerCore(
            this IServiceCollection services)
            => services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}
