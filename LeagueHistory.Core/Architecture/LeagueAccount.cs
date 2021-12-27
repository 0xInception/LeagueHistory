﻿namespace LeagueHistory.Core.Architecture
{
    public class LeagueAccount
    {
        public LeagueCredentials Credentials { get; set; }
        public AccessToken AccessToken { get; set; }

        public LeagueAccount(LeagueCredentials credentials)
        {
            Credentials = credentials;
        }
    }
}