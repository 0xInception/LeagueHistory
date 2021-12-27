namespace LeagueHistory.Core
{
    // Unsure how we should handle regions, will be clear in the future.
    public class LeagueCredentials
    {
        public string Username { get; }
        public string Password { get; }
        public Region Region { get; set; }
        public LeagueCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}