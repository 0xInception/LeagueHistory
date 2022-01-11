namespace LeagueHistory.Core
{
    public class LeagueAccount
    {
        public LeagueCredentials Credentials { get; set; }
        public AuthenticationInfo Info { get; set; } = new();
        public BlueToken BlueToken { get; set; } = new();

        public LeagueAccount(LeagueCredentials credentials)
        {
            Credentials = credentials;
        }
    }

    public class AuthenticationInfo
    {
        public string Entitlements { get; set; }
        public string UserInfo { get; set; }
        public AccessToken? AccessToken { get; set; }
    }

    public class BlueToken
    {
        public string LoginToken { get; set; }
        public string SessionToken { get; set; }
    }
}