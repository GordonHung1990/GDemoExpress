using MediatR;

namespace GDemoExpress.Core.ApplicationServices
{
    public record PlayerQueryRequest : IRequest<IEnumerable<PlayerDataResponse>?>
    {
    }
}
