namespace LeagueHistory.Core
{
    public class LeagueAccount
    {
        public LeagueCredentials Credentials { get; set; }
        public AccessToken? AccessToken { get; set; } = null!;

        public LeagueAccount(LeagueCredentials credentials)
        {
            Credentials = credentials;
        }
    }
}