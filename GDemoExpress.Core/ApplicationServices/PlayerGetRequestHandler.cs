using MediatR;
using Microsoft.Extensions.Logging;

namespace GDemoExpress.Core.ApplicationServices
{
    internal class PlayerGetRequestHandler : IRequestHandler<PlayerGetRequest, PlayerDataResponse?>
    {
        private readonly ILogger<PlayerGetRequestHandler> _logger;
        private readonly IPlayer _player;

        public PlayerGetRequestHandler(
            IPlayer player,
            ILogger<PlayerGetRequestHandler> logger)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PlayerDataResponse?> Handle(PlayerGetRequest request, CancellationToken cancellationToken)
        {
            var player = await _player.GetAsync(
                request.PlayerId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTimeOffset.UtcNow.ToString("O"), request, player);

            return player is null
                ? null
                : new PlayerDataResponse(
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
