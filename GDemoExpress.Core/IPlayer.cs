using GDemoExpress.Core.Models;

namespace GDemoExpress.Core
{
    public interface IPlayer
    {
        ValueTask<Guid> AddAsync(PlayerAdd player, CancellationToken cancellationToken = default);

        ValueTask<PlayerData?> GetAsync(Guid playerId, CancellationToken cancellationToken = default);

        ValueTask<PlayerData?> GetAsync(string account, CancellationToken cancellationToken = default);

        ValueTask<Guid?> GetByIdAsync(string account, CancellationToken cancellationToken = default);

        IAsyncEnumerable<PlayerData> QueryAsync(CancellationToken cancellationToken = default);

        ValueTask UpdateByPasswordAsync(PlayerUpdateByPassword player, CancellationToken cancellationToken = default);

        ValueTask UpdateByStatusAsync(PlayerUpdateByStatus player, CancellationToken cancellationToken = default);
    }
}
