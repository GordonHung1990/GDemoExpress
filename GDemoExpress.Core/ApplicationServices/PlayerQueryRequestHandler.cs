using MediatR;
using Microsoft.Extensions.Logging;

namespace GDemoExpress.Core.ApplicationServices
{
    internal class PlayerQueryRequestHandler : IRequestHandler<PlayerQueryRequest, IEnumerable<PlayerDataResponse>?>
    {
        private readonly ILogger<PlayerQueryRequestHandler> _logger;
        private readonly IPlayer _player;

        public PlayerQueryRequestHandler(
            IPlayer player,
            ILogger<PlayerQueryRequestHandler> logger)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<PlayerDataResponse>?> Handle(PlayerQueryRequest request, CancellationToken cancellationToken)
        {
            var players = await _player.QueryAsync(cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                    DateTimeOffset.UtcNow.ToString("O"), request, players);

            return players == null || !players.Any()
                ? Enumerable.Empty<PlayerDataResponse>()
                : players.Select(player => new PlayerDataResponse(
                    PlayerId: player.PlayerId,
                    Account: player.Account,
                    Status: player.Status,
                    LastName: player.LastName,
                    FullName: player.FullName,
                    NickName: player.NickName,
                    PhoneNumber: player.PhoneNumber,
                    Mailbox: player.Mailbox,
                    CreatedOn: player.CreatedOn,
                    UpdatedOn: player.UpdatedOn)).AsEnumerable();
        }
    }
}
