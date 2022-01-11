using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LeagueHistory.Core.Enums;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{
    public class AccountPool : IAccountPool
    {
        public AccountPool(ILeagueAuthenticator authenticator,IBlueAuthenticator blueAuthenticator, ILogger logger, ISettingsProvider settings)
        {
            Authenticator = authenticator;
            BlueAuthenticator = blueAuthenticator;
            Logger = logger;
            Accounts = settings.ReadSettings()!.Accounts;
            Timer = new System.Timers.Timer(1800000);
            Timer.Elapsed += OnTimerOnElapsed;
            Timer.Start();
            Authenticate();
        }
        public System.Timers.Timer Timer { get; set; }
        public ILeagueAuthenticator Authenticator { get; }
        public IBlueAuthenticator BlueAuthenticator { get; }
        public ILogger Logger { get; }
        public string[] Accounts { get; set; }
        public List<LeagueAccount> ActiveAccounts { get; set; } = new();

        public BlueToken? GetAccount(Region region)
        {
            var x = ActiveAccounts.FirstOrDefault(d => d.Info.AccessToken!.Region == region);
            return x?.BlueToken;
        }

        private void OnTimerOnElapsed(object x, ElapsedEventArgs y) 
            => Authenticate();

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
                        if (BlueAuthenticator.Authenticate(leagueAccount).Result == Result.Valid)
                        {
                            ActiveAccounts.Add(leagueAccount);
                            Logger.Success(
                                $"Successfully authenticated account [{leagueAccount.Credentials.Username}:{leagueAccount.Credentials.Password}]");
                        }
                        else
                        {
                            Logger.Error(
                                $"Failed to blue authenticate account [{leagueAccount.Credentials.Username}:{leagueAccount.Credentials.Password}]");
                        }
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
                        if (BlueAuthenticator.Authenticate(account).Result == Result.Valid)
                        {
                            ActiveAccounts.Add(account);
                            Logger.Success(
                                $"Successfully authenticated account [{account.Credentials.Username}:{account.Credentials.Password}]");
                        }
                        else
                        {
                            Logger.Error(
                                $"Failed to blue authenticate account [{account.Credentials.Username}:{account.Credentials.Password}]");
                        }
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