using GDemoExpress.Core.Models;

namespace GDemoExpress.Core
{
    public interface IPlayer
    {
        public ValueTask<Guid> AddAsync(PlayerAdd player);

        public ValueTask<PlayerGet?> GetAsync(Guid playerId);

        public ValueTask<PlayerGet?> GetAsync(string account);

        public ValueTask<Guid?> GetByIdAsync(string account);

        public ValueTask UpdateByPasswordAsync(PlayerUpdateByPassword player);

        public ValueTask UpdateByStatusAsync(PlayerUpdateByStatus player);
    }
}
