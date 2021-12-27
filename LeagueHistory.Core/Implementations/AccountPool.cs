using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{


    public class AccountPool : IAccountPool
    {
        public System.Timers.Timer Timer { get; set; }
        public ILeagueAuthenticator Authenticator { get; }
        public ILogger Logger { get; }
        public string[] Accounts { get; set; }
        public List<LeagueAccount> ActiveAccounts { get; set; } = new();
        public bool FirstRun { get; set; } = true;

        public AccountPool(ILeagueAuthenticator authenticator, ILogger logger, ISettingsProvider settings)
        {
            Authenticator = authenticator;
            Logger = logger;
            Accounts = settings.ReadSettings()!.Accounts;
            Timer = new System.Timers.Timer(1800000);
            Timer.Elapsed += OnTimerOnElapsed;
            Timer.Start();
            Authenticate();
        }

        public AccessToken GetAccount(Region region) // TODO: Figure out how to do the region stuff
        {
            return ActiveAccounts.First(d => d.AccessToken.Region == region).AccessToken;
        }

        private async void OnTimerOnElapsed(object x, ElapsedEventArgs y)
        {
            Authenticate();
        }

        private void Authenticate()
        {
            if (ActiveAccounts.Count == 0)
            {
                foreach (var account in Accounts)
                {
                    var split = account.Split(':');
                    var leagueAccount = new LeagueAccount(new LeagueCredentials(split[0], split[1]));
                    if (Authenticator.Authenticate(leagueAccount).Result == Result.Valid)
                    {
                        Logger.Success(
                            $"Successfully authenticated account [{leagueAccount.Credentials.Username}:{leagueAccount.Credentials.Password}]");
                    }
                    else
                    {
                        Logger.Error(
                            $"Failed to authenticate account [{leagueAccount.Credentials.Username}:{leagueAccount.Credentials.Password}]");
                    }
                }
            }
            else // TODO: Refresh token
            {
                foreach (var account in ActiveAccounts)
                {
                    if (Authenticator.Authenticate(account).Result == Result.Valid)
                    {
                        ActiveAccounts.Add(account);
                        Logger.Success(
                            $"Successfully authenticated account [{account.Credentials.Username}:{account.Credentials.Password}]");
                    }
                    else
                    {
                        Logger.Error(
                            $"Failed to authenticate account [{account.Credentials.Username}:{account.Credentials.Password}]");
                    }
                }
            }
        }
    }
}