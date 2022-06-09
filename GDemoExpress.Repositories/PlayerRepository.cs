using GDemoExpress.Core;
using GDemoExpress.Core.Models;
using GDemoExpress.DataBase.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NETCore.Encrypt.Extensions;

namespace GDemoExpress.Repositories
{
    internal class PlayerRepository : IPlayer
    {
        private readonly DboContext _context;
        private readonly DemoExpressOptions _demoExpress;
        private readonly ILogger<PlayerRepository> _logger;

        public PlayerRepository(DboContext context,
            ILogger<PlayerRepository> logger,
            IOptions<DemoExpressOptions> DemoExpressOptions)
        {
            _demoExpress = DemoExpressOptions.Value;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<Guid> AddAsync(PlayerAdd player, CancellationToken cancellationToken = default)
        {
            var playerId = NewId.NextGuid();
            var hashed = player.Password.HMACSHA512(_demoExpress.PlayerCryptographyKey);
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _ = _context.Players.Add(new Player()
                {
                    PlayerId = playerId,
                    Account = player.Account.ToLower(),
                    Password = hashed,
                    Status = (int)PlayerStatus.ENABLE,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                });
                _ = await _context.SaveChangesAsync().ConfigureAwait(false);
                _ = _context.Playerinfos.Add(new Playerinfo()
                {
                    PlayerId = playerId,
                    NickName = player.NickName
                });
                _ = await _context.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
                return playerId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "This player creation failed.");
                throw new Exception($"This player creation failed.({player.Account})");
            }
        }

        public async ValueTask<PlayerData?> GetAsync(Guid playerId, CancellationToken cancellationToken = default)
            => await (from player in _context.Players.AsQueryable()
                      join playerinfo in _context.Playerinfos.AsQueryable()
                      on player.PlayerId equals playerinfo.PlayerId
                      where player.PlayerId == playerId
                      select new PlayerData(
                         player.PlayerId,
                         player.Account,
                         (PlayerStatus)player.Status,
                         playerinfo.LastName,
                         playerinfo.FullName,
                         playerinfo.NickName,
                         playerinfo.PhoneNumber,
                         playerinfo.Mailbox,
                         player.CreatedOn,
                         player.UpdatedOn))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

        public async ValueTask<PlayerData?> GetAsync(string account, CancellationToken cancellationToken = default)
        {
            var playerId = await GetByIdAsync(account).ConfigureAwait(false);
            return !playerId.HasValue ? null : await GetAsync(playerId.Value).ConfigureAwait(false);
        }

        public async ValueTask<Guid?> GetByIdAsync(string account, CancellationToken cancellationToken = default)
            => await _context.Players.AsQueryable()
            .Where(x => x.Account == account.ToLower())
            .Select(x => x.PlayerId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        public IAsyncEnumerable<PlayerData> QueryAsync(CancellationToken cancellationToken = default)
            => _context.Players.AsQueryable()
                .Join(_context.Playerinfos.AsQueryable(),
                    player => player.PlayerId,
                    playerinfo => playerinfo.PlayerId,
                    (player, playerinfo) => new PlayerData(
                        player.PlayerId,
                        player.Account,
                        (PlayerStatus)player.Status,
                        playerinfo.LastName,
                        playerinfo.FullName,
                        playerinfo.NickName,
                        playerinfo.PhoneNumber,
                        playerinfo.Mailbox,
                        player.CreatedOn,
                        player.UpdatedOn)
                    ).ToAsyncEnumerable();

        public ValueTask UpdateByPasswordAsync(PlayerUpdateByPassword player, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public ValueTask UpdateByStatusAsync(PlayerUpdateByStatus player, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
