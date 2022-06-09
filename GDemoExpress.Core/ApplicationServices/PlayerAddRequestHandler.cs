using GDemoExpress.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GDemoExpress.Core.ApplicationServices
{
    internal class PlayerAddRequestHandler : IRequestHandler<PlayerAddRequest, Guid>
    {
        private readonly ILogger<PlayerAddRequestHandler> _logger;
        private readonly IPlayer _player;

        public PlayerAddRequestHandler(
            IPlayer player,
            ILogger<PlayerAddRequestHandler> logger)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(PlayerAddRequest request, CancellationToken cancellationToken)
        {
            var playerId = await _player.AddAsync(new PlayerAdd(
                request.Account,
                request.Password,
                request.NickName)).ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTimeOffset.UtcNow.ToString("O"), request, playerId);
            return playerId;
        }
    }
}
