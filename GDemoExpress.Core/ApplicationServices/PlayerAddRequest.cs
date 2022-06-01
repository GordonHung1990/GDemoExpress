using MediatR;

namespace GDemoExpress.Core.ApplicationServices
{
    public record PlayerAddRequest : IRequest<Guid>
    {
        public string Account { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string NickName { get; init; } = default!;
    }
}
