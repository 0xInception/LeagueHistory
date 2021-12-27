using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using LeagueHistory.Core.Architecture;
using LeagueHistory.Core.Architecture.Interfaces;

namespace LeagueHistory.Core.Architecture.Implementations
{
    

    public class AccountPool : IAccountPool
    {
        public System.Timers.Timer Timer { get; set; }
        public ILeagueAuthenticator Authenticator { get; }
        public ILogger Logger { get; }
        public string[] Accounts { get; set; }
        public List<LeagueAccount> ActiveAccounts { get; set; } = new();
        public bool FirstRun { get; set; } = true;

        public AccountPool(ILeagueAuthenticator authenticator,ILogger logger,ISettingsProvider settings)
        {
            Authenticator = authenticator;
            Logger = logger;
            Accounts = settings.ReadSettings()!.Accounts;
            Timer = new System.Timers.Timer(1800000);
            Timer.Elapsed += OnTimerOnElapsed;
            Timer.Start();
            Authenticate();
        }

        public void GetAccount(Region region) // TODO: Figure out how to do the region stuff
        {
            throw new NotImplementedException();
        }
        private async void OnTimerOnElapsed(object x, ElapsedEventArgs y)
        { 
            Authenticate();
        }

        private async void Authenticate()
        {
            if (ActiveAccounts.Count == 0)
            {
                foreach (var account in Accounts)
                {
                    var split = account.Split(':');
                    var leagueAccount = new LeagueAccount(new LeagueCredentials(split[0], split[1]));
                    if (await Authenticator.Authenticate(leagueAccount) == Result.Valid)
                    {
                        ActiveAccounts.Add(leagueAccount);
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
                    if (await Authenticator.Authenticate(account) == Result.Valid)
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