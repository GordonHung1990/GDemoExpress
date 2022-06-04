using MediatR;
using Microsoft.Extensions.Logging;

namespace GDemoExpress.Core.ApplicationServices
{
    internal class PlayerGetCommand : IRequestHandler<PlayerGetRequest, PlayerGetResponse?>
    {
        private readonly ILogger<PlayerGetCommand> _logger;
        private readonly IPlayer _player;

        public PlayerGetCommand(
            IPlayer player,
            ILogger<PlayerGetCommand> logger)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PlayerGetResponse?> Handle(PlayerGetRequest request, CancellationToken cancellationToken)
        {
            var player = await _player.GetAsync(request.PlayerId).ConfigureAwait(false);
            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTimeOffset.UtcNow.ToString("O"), request, player);
            return player is null
                ? null
                : new PlayerGetResponse(
                    PlayerId: player.PlayerId,
                    Account: player.Account,
                    Status: player.Status,
                    LastName: player.LastName,
                    FullName: player.FullName,
                    NickName: player.NickName,
                    PhoneNumber: player.PhoneNumber,
                    Mailbox: player.Mailbox,
                    CreatedOn: player.CreatedOn,
                    UpdatedOn: player.UpdatedOn);
        }
    }
}
