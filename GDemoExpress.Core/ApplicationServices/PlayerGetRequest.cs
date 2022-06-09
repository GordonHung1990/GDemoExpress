using MediatR;

namespace GDemoExpress.Core.ApplicationServices
{
    public record PlayerGetRequest : IRequest<PlayerDataResponse?>
    {
        public Guid PlayerId { get; init; }
    }
}
