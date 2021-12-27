using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueHistory.Core.Interfaces
{
    public interface ILeagueAuthenticator
    {
        HttpClient AuthenticatorClient { get; set; }
        Task<Result> Authenticate(LeagueAccount account);
        Task<Result> Refresh(LeagueAccount account);
        Task<Result> ResolveRegion(LeagueAccount account);
    }
}