using System.Text.Json;
using StackExchange.Redis;
using GDemoExpress.Core.Models;
using GDemoExpress.Core;

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

        public ValueTask<Guid> AddAsync(PlayerAdd player)
        => _player.AddAsync(player);

        public async ValueTask<PlayerGet?> GetAsync(string account)
        {
            var playerId = await GetByIdAsync(account).ConfigureAwait(false);
            return !playerId.HasValue ? null : await GetAsync(playerId.Value).ConfigureAwait(false);
        }

        public async ValueTask<PlayerGet?> GetAsync(Guid playerId)
        {
            var key = string.Format(KeyPlayerGet, playerId);
            PlayerGet? playerGet;
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                playerGet = await _player.GetAsync(playerId).ConfigureAwait(false);

                _ = await _database.StringSetAsync(
                key,
                JsonSerializer.Serialize(playerGet),
                expiry: _expiry,
                when: When.NotExists).ConfigureAwait(false);

                return playerGet;
            }
            playerGet = JsonSerializer.Deserialize<PlayerGet>(await _database.StringGetAsync(key).ConfigureAwait(false));

            return playerGet;
        }

        public async ValueTask<Guid?> GetByIdAsync(string account)
        {
            var key = string.Format(KeyPlayerAccount, account);

            Guid? playerId;
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                playerId = await _player.GetByIdAsync(account).ConfigureAwait(false);
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

        public async ValueTask UpdateByPasswordAsync(PlayerUpdateByPassword player)
        => await _player.UpdateByPasswordAsync(player).ConfigureAwait(false);

        public async ValueTask UpdateByStatusAsync(PlayerUpdateByStatus player)
        {
            await _player.UpdateByStatusAsync(player).ConfigureAwait(false);

            var playerGet = await _player.GetAsync(player.PlayerId).ConfigureAwait(false);
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
