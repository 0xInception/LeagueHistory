namespace LeagueHistory.Core.JsonObjects
{
    public class LookupResponse
    {
        public bool Valid { get; set; }
        public string Region { get; set; }
        public long id { get; set; }
        public string puuid { get; set; }
        public long accountId { get; set; }
        public string name { get; set; }
        public object gameNameTagline { get; set; }
        public int profileIconId { get; set; }
        public int level { get; set; }
        public int expPoints { get; set; }
        public int levelAndXpVersion { get; set; }
        public int revisionId { get; set; }
        public long revisionDate { get; set; }
        public long lastGameDate { get; set; }
        public bool nameChangeFlag { get; set; }
        public bool unnamed { get; set; }
        public string privacy { get; set; }
        public int expToNextLevel { get; set; }
    }
}