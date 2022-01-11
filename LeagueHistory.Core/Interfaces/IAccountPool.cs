using LeagueHistory.Core.Enums;

namespace LeagueHistory.Core.Interfaces
{
    public interface IAccountPool
    {
        BlueToken? GetAccount(Region region);
    }
}