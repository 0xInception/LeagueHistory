using System.Net.Http;
using System.Threading.Tasks;
using LeagueHistory.Core.Architecture;

namespace LeagueHistory.Core
{
    public interface ILeagueAuthenticator
    {
        HttpClient AuthenticatorClient { get; set; }
        Task<Result> Authenticate(LeagueAccount account);
        Task<Result> Refresh(LeagueAccount account);
    }
}