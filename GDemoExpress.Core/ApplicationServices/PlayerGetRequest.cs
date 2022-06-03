using MediatR;

namespace GDemoExpress.Core.ApplicationServices
{
    public record PlayerGetRequest : IRequest<PlayerGetResponse?>
    {
        public Guid PlayerId { get; init; }
    }
}
