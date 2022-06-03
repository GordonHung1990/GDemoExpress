using GDemoExpress.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GDemoExpress.Core.ApplicationServices
{
    internal class PlayerAddCommand : IRequestHandler<PlayerAddRequest, Guid>
    {
        private readonly ILogger<PlayerAddCommand> _logger;
        private readonly IPlayer _player;

        public PlayerAddCommand(
            IPlayer player,
            ILogger<PlayerAddCommand> logger)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<Guid> Handle(PlayerAddRequest request, CancellationToken cancellationToken)
            => _player.AddAsync(new PlayerAdd(
                request.Account,
                request.Password,
                request.NickName)).AsTask();
    }
}
