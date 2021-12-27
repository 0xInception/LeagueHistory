using System;
using System.Net.Http;
using LeagueHistory.Core.Architecture.Interfaces;
using LeagueHistory.Core.Architecture.JsonObjects;

namespace LeagueHistory.Core.Architecture.Implementations
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

        public LookupResponse Lookup(string username, Region region)
        {
            throw new NotImplementedException();
        }

        public LookupResponse LookupPuuid(string puuid, Region region)
        {
            throw new NotImplementedException();
        }
    }
}