using LeagueHistory.Core.Architecture.JsonObjects;

namespace LeagueHistory.Core.Architecture.Interfaces
{
    public interface ILeagueApi
    {
        LookupResponse Lookup(string username, Region region);
        LookupResponse LookupPuuid(string puuid, Region region);
    }
}