using System.Threading.Tasks;
using LeagueHistory.Core.JsonObjects;

namespace LeagueHistory.Core.Interfaces
{
    public interface ILeagueApi
    {
        Task<LookupResponse?> Lookup(string username, Region region);
        Task<LookupResponse> LookupPuuid(string puuid, Region region);
    }
}