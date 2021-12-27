using LeagueHistory.Core.JsonObjects;

namespace LeagueHistory.Core.Interfaces
{
    public interface ILeagueApi
    {
        LookupResponse Lookup(string username, Region region);
        LookupResponse LookupPuuid(string puuid, Region region);
    }
}