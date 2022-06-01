using MediatR;
using GDemoExpress.Core.ApplicationServices;

namespace GDemoExpress.Core.ApplicationServices
{
    public record PlayerGetRequest : IRequest<PlayerGetResponse?>
    {
        public Guid PlayerId { get; init; }
    }
}
