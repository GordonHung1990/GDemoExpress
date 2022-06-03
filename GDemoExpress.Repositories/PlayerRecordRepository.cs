using System.Text.Json;
using GDemoExpress.Core;
using GDemoExpress.Core.Models;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GDemoExpress.Repositories
{
    public class PlayerRecordRepository : IPlayer
    {
        private readonly IMongoDatabase _database;
        private readonly IPlayer _player;

        public PlayerRecordRepository(
            IMongoDatabase database,
            IPlayer player)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public async ValueTask<Guid> AddAsync(PlayerAdd player)
        {
            var playerId = await _player.AddAsync(player).ConfigureAwait(false);

            var playerInfo = await _player.GetAsync(playerId).ConfigureAwait(false);

            if (playerInfo == null)
            {
                return playerId;
            }

            await AddAsync(new PlayerRecordAdd(
                PlayerId: playerInfo.PlayerId,
                Account: playerInfo.Account,
                OperationType: OperationType.CREATE,
                OldData: JsonSerializer.Serialize(new { }),
                NewData: JsonSerializer.Serialize(playerInfo))).ConfigureAwait(false);

            return playerId;
        }

        public ValueTask<PlayerGet?> GetAsync(Guid playerId)
           => _player.GetAsync(playerId);

        public ValueTask<PlayerGet?> GetAsync(string account)
            => _player.GetAsync(account);

        public ValueTask<Guid?> GetByIdAsync(string account)
            => _player.GetByIdAsync(account);

        public async ValueTask UpdateByPasswordAsync(PlayerUpdateByPassword player)
        {
            var playerInfo = await _player.GetAsync(player.PlayerId).ConfigureAwait(false);

            if (playerInfo == null)
            {
                throw new Exception("player does not exist");
            }
            await _player.UpdateByPasswordAsync(player).ConfigureAwait(false);

            await AddAsync(new PlayerRecordAdd(
                PlayerId: playerInfo.PlayerId,
                Account: playerInfo.Account,
                OperationType: OperationType.UPDATE_PASSWORD,
                OldData: JsonSerializer.Serialize(new { }),
                NewData: JsonSerializer.Serialize(new { }))).ConfigureAwait(false);
        }

        public async ValueTask UpdateByStatusAsync(PlayerUpdateByStatus player)
        {
            var playerInfo = await _player.GetAsync(player.PlayerId).ConfigureAwait(false);

            if (playerInfo == null)
            {
                throw new Exception("player does not exist");
            }
            await _player.UpdateByStatusAsync(player).ConfigureAwait(false);

            await AddAsync(new PlayerRecordAdd(
                PlayerId: playerInfo.PlayerId,
                Account: playerInfo.Account,
                OperationType: OperationType.UPDATE_STATUS,
                OldData: JsonSerializer.Serialize(new { playerInfo.Status }),
                NewData: JsonSerializer.Serialize(new { player.Status }))).ConfigureAwait(false);
        }

        private static Task CreatedOnIndexAsync(IMongoCollection<PlayerRecord> collection)
        {
            var index = Builders<PlayerRecord>.IndexKeys
                .Ascending(m => m.OperationOn);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_operation_on"
            };
            var createIndexModel = new CreateIndexModel<PlayerRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private static Task PlayerAccountIndexAsync(IMongoCollection<PlayerRecord> collection)
        {
            var index = Builders<PlayerRecord>.IndexKeys
               .Ascending(m => m.Account);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_account"
            };
            var createIndexModel = new CreateIndexModel<PlayerRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private static Task PlayerIdIndexAsync(IMongoCollection<PlayerRecord> collection)
        {
            var index = Builders<PlayerRecord>.IndexKeys
               .Ascending(m => m.PlayerId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_player_id"
            };
            var createIndexModel = new CreateIndexModel<PlayerRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private static Task PrimaryKeyIndexAsync(IMongoCollection<PlayerRecord> collection)
        {
            var index = Builders<PlayerRecord>.IndexKeys
               .Ascending(m => m.PlayerRecordId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"pk_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}"
            };
            var createIndexModel = new CreateIndexModel<PlayerRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private async Task AddAsync(PlayerRecordAdd add)
        {
            var collection = await GetCollectionAsync().ConfigureAwait(false);
            var playerRecordId = NewId.NextGuid();
            await collection.InsertOneAsync(new PlayerRecord()
            {
                PlayerRecordId = playerRecordId,
                PlayerId = add.PlayerId,
                Account = add.Account,
                OldData = add.OldData,
                NewData = add.NewData,
                OperationOn = DateTimeOffset.UtcNow
            }).ConfigureAwait(false);
        }

        private async ValueTask<bool> CollectionExistsAsync(string collectionName)
            => await (await _database.ListCollectionsAsync(new ListCollectionsOptions { Filter = new BsonDocument("name", collectionName) }).ConfigureAwait(false)).AnyAsync().ConfigureAwait(false);

        private async Task<IMongoCollection<PlayerRecord>> GetCollectionAsync()
        {
            var collection = _database.GetCollection<PlayerRecord>(typeof(PlayerRecord).Name);
            if (collection.CollectionNamespace == null)
            {
                return collection;
            }

            if (!await CollectionExistsAsync(collection.CollectionNamespace.CollectionName.ToString()).ConfigureAwait(false))
            {
                await PrimaryKeyIndexAsync(collection).ConfigureAwait(false);
                await CreatedOnIndexAsync(collection).ConfigureAwait(false);
                await PlayerIdIndexAsync(collection).ConfigureAwait(false);
                await PlayerAccountIndexAsync(collection).ConfigureAwait(false);
            }

            return collection;
        }
    }
}
