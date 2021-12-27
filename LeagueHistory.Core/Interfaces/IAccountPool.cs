using LeagueHistory.Core.Enums;

namespace LeagueHistory.Core.Interfaces
{
    public interface IAccountPool
    {
        AccessToken GetAccount(Region region);
    }
}