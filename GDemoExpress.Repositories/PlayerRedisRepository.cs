using System.Text.Json;
using GDemoExpress.Core;
using GDemoExpress.Core.Models;
using StackExchange.Redis;

namespace GDemoExpress.Repositories
{
    public class PlayerRedisRepository : IPlayer
    {
        private const string KeyPlayerAccount = "player:account:{0}";
        private const string KeyPlayerGet = "player:get:{0}";
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly IPlayer _player;

        public PlayerRedisRepository(
            IDatabase database,
            IPlayer player,
            TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _expiry = expiry;
        }

        public ValueTask<Guid> AddAsync(PlayerAdd player, CancellationToken cancellationToken = default)
        => _player.AddAsync(player, cancellationToken: cancellationToken);

        public async ValueTask<PlayerData?> GetAsync(string account, CancellationToken cancellationToken = default)
        {
            var playerId = await GetByIdAsync(account, cancellationToken: cancellationToken).ConfigureAwait(false);
            return playerId.HasValue ? null : await GetAsync(playerId.Value, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<PlayerData?> GetAsync(Guid playerId, CancellationToken cancellationToken = default)
        {
            var key = string.Format(KeyPlayerGet, playerId);
            PlayerData? playerGet;
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                playerGet = await _player.GetAsync(playerId, cancellationToken: cancellationToken).ConfigureAwait(false);

                _ = await _database.StringSetAsync(
                key,
                JsonSerializer.Serialize(playerGet),
                expiry: _expiry,
                when: When.NotExists).ConfigureAwait(false);

                return playerGet;
            }
            playerGet = JsonSerializer.Deserialize<PlayerData>(await _database.StringGetAsync(key).ConfigureAwait(false));

            return playerGet;
        }

        public async ValueTask<Guid?> GetByIdAsync(string account, CancellationToken cancellationToken = default)
        {
            var key = string.Format(KeyPlayerAccount, account);

            Guid? playerId;
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                playerId = await _player.GetByIdAsync(account, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (!playerId.HasValue)
                {
                    return playerId;
                }
                _ = await _database.StringSetAsync(
                key,
                playerId.ToString(),
                when: When.NotExists).ConfigureAwait(false);
                return playerId;
            }
            playerId = Guid.Parse(await _database.StringGetAsync(key).ConfigureAwait(false));

            return playerId;
        }

        public IAsyncEnumerable<PlayerData> QueryAsync(CancellationToken cancellationToken = default)
            => _player.QueryAsync(cancellationToken: cancellationToken);

        public async ValueTask UpdateByPasswordAsync(PlayerUpdateByPassword player, CancellationToken cancellationToken = default)
        => await _player.UpdateByPasswordAsync(player, cancellationToken: cancellationToken).ConfigureAwait(false);

        public async ValueTask UpdateByStatusAsync(PlayerUpdateByStatus player, CancellationToken cancellationToken = default)
        {
            await _player.UpdateByStatusAsync(player, cancellationToken: cancellationToken).ConfigureAwait(false);

            var playerGet = await _player.GetAsync(player.PlayerId, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (playerGet is null)
            {
                return;
            }
            _ = await _database.StringSetAsync(
            string.Format(KeyPlayerGet, playerGet.Account),
            JsonSerializer.Serialize(playerGet),
            expiry: _expiry,
            when: When.Exists).ConfigureAwait(false);
        }
    }
}
