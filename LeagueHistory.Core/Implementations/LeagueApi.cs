using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueHistory.Core.Interfaces;
using LeagueHistory.Core.JsonObjects;

namespace LeagueHistory.Core.Implementations
{
    

    public class LeagueApi : ILeagueApi
    {
        public IAccountPool AccountPool { get; }
        public HttpClient ApiClient { get; set; }

        public LeagueApi(ILeagueAuthenticator authenticator, IAccountPool accountPool)
        {
            AccountPool = accountPool;
            ApiClient = authenticator
                .AuthenticatorClient; // Hack to re-use httpclient instance, maybe consider a different way
        }

        public async Task<LookupResponse?> Lookup(string username, Region region)
        {
            var account = AccountPool.GetAccount(region);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, ConstructBlueCall(region,$"/summoner-ledge/v1/regions/{region.ToPlatform()}/summoners/name/{username}"));
            requestMessage.Headers.Add("Authorization",$"{account.AccessToken.token_type} {account.AccessToken.access_token}"); // TODO: Apparently ledge is not using access_token anymore, but some other jwt token issued from https://session.gpsrv.pvp.net
            var response = await ApiClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LookupResponse>(stringResponse);
            }
            return new LookupResponse()
            {
                Valid = false
            };
        }

        public async Task<LookupResponse> LookupPuuid(string puuid, Region region)
        {
            throw new NotImplementedException();
        }
        private string ConstructBlueCall(Region region,string call)
        {
            var builder = new StringBuilder();
            builder.Append("https://");
            builder.Append(region);
            builder.Append("-blue.lol.sgp.pvp.net");
            builder.Append(call);
            return builder.ToString();
        }
    }
}