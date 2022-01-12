using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueHistory.Core.Enums;
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
            if (account is null)
            {
                return new LookupResponse()
                {
                    Valid = false
                };
            }
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, ConstructBlueCall(region,$"/summoner-ledge/v1/regions/{region.ToPlatform()}/summoners/name/{username}"));
            requestMessage.Headers.Add("Accept","application/json");
            requestMessage.Headers.Add("User-Agent","RiotClient/18.0.0 (rso-auth)");
            requestMessage.Headers.Add("Authorization",$"Bearer {account.SessionToken}");
            var response = await ApiClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                var resp = JsonSerializer.Deserialize<LookupResponse>(stringResponse);
                if (resp != null)
                {
                    resp.Valid = true;
                    return resp;
                }
            }
            return new LookupResponse()
            {
                Valid = true
            };
        }

        public async Task<LookupResponse> LookupPuuid(string puuid, Region region)
        {
            var account = AccountPool.GetAccount(region);
            if (account is null)
            {
                return new LookupResponse()
                {
                    Valid = false,
                    Region = region.ToString()
                };
            }

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, ConstructBlueCall(region,$"/summoner-ledge/v1/regions/{region.ToPlatform()}/summoners/puuid/{puuid}"));
            requestMessage.Headers.Add("Accept","application/json");
            requestMessage.Headers.Add("User-Agent","RiotClient/18.0.0 (rso-auth)");
            requestMessage.Headers.Add("Authorization",$"Bearer {account.SessionToken}");
            var response = await ApiClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                var resp = JsonSerializer.Deserialize<LookupResponse>(stringResponse);
                if (resp != null)
                {
                    resp.Valid = true;
                    resp.Region = region.ToString();
                    return resp;
                }
            }
            return new LookupResponse()
            {
                Valid = true,
                Region = region.ToString()
            };
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