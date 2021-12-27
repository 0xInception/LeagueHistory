namespace LeagueHistory.Core.Interfaces
{
    public interface IAccountPool
    {
        AccessToken GetAccount(Region region);
    }
}