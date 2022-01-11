using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueHistory.Core.Enums;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{
    public class BlueAuthenticator : IBlueAuthenticator
    {
        public BlueAuthenticator(IRandomProvider randomProvider)
        {
            RandomProvider = randomProvider;
            AuthenticatorClient = new HttpClient(new HttpClientHandler()
            {
                UseCookies = false
            });
        }

        public HttpClient AuthenticatorClient { get; set; }
        public IRandomProvider RandomProvider { get; }

        public async Task<Result> Authenticate(LeagueAccount account)
        {
            var loginToken = await PostLoginToken(account);
            if (loginToken.result != Result.Valid)
                return loginToken.result;

            account.BlueToken.LoginToken = loginToken.token;
            var sessionToken = await CreateSession(account);
            if (sessionToken.result != Result.Valid)
                return sessionToken.result;
            account.BlueToken.SessionToken = sessionToken.token;
            return Result.Valid;
        }

        public async Task<Result> Refresh(LeagueAccount account)
        {
            var refresh = await RefreshSessionToken(account);
            if (refresh.result != Result.Valid)
                return refresh.result;
            account.BlueToken.SessionToken = refresh.token;
            return Result.Valid;
        }

        private async Task<(Result result, string token)> RefreshSessionToken(LeagueAccount account)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                ConstructLoginToken(account.Info.AccessToken.Region, "/session-external/v1/session/refresh"));
            requestMessage.Headers.Add("Authorization",
                $"{account.Info.AccessToken.token_type} {account.BlueToken.LoginToken}");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "LeagueOfLegendsClient/12.1.416.5961");

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{\"lst\":\"");
            stringBuilder.Append(account.BlueToken.SessionToken);
            stringBuilder.Append("\"}");
            requestMessage.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "application/json");
            var part1 = await AuthenticatorClient.SendAsync(requestMessage);
            if (!part1.IsSuccessStatusCode)
                return (Result.Unknown, string.Empty);
            var response = await part1.Content.ReadAsStringAsync();
            return (Result.Valid, response.Trim('"'));
        }
        private async Task<(Result result, string token)> PostLoginToken(LeagueAccount account)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                ConstructLoginToken(account.Info.AccessToken.Region,
                    $"/login-queue/v2/login/products/lol/regions/{account.Info.AccessToken.Region.ToPlatform()}"));
            requestMessage.Headers.Add("Authorization",
                $"{account.Info.AccessToken.token_type} {account.Info.AccessToken.access_token}");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "LeagueOfLegendsClient/12.1.416.5961");
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{\"clientName\":\"lcu\",\"entitlements\":\"");
            stringBuilder.Append(account.Info.Entitlements);
            stringBuilder.Append("\",\"userinfo\":\"");
            stringBuilder.Append(account.Info.UserInfo);
            stringBuilder.Append("\"}");
            requestMessage.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "application/json");
            var part1 = await AuthenticatorClient.SendAsync(requestMessage);
            if (!part1.IsSuccessStatusCode)
                return (Result.Unknown, string.Empty);
            var response = await part1.Content.ReadAsStringAsync();
            if (!response.Contains("token"))
                return (Result.Unknown, string.Empty);

            var json = JsonDocument.Parse(response);
            return (Result.Valid, json.RootElement.GetProperty("token").GetString()!);
        }

        private async Task<(Result result, string token)> CreateSession(LeagueAccount account)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                ConstructLoginToken(account.Info.AccessToken.Region, "/session-external/v1/session/create"));
            requestMessage.Headers.Add("Authorization",
                $"{account.Info.AccessToken.token_type} {account.BlueToken.LoginToken}");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "LeagueOfLegendsClient/12.1.416.5961");

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{\"claims\":{\"cname\":\"lcu\"},\"product\":\"lol\",\"puuid\":\"");
            stringBuilder.Append(account.Info.AccessToken.Puuid);
            stringBuilder.Append("\",\"region\":\"");
            stringBuilder.Append(account.Info.AccessToken.Region.ToPlatform());
            stringBuilder.Append("\"}");
            requestMessage.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "application/json");
            var part1 = await AuthenticatorClient.SendAsync(requestMessage);
            if (!part1.IsSuccessStatusCode)
                return (Result.Unknown, string.Empty);
            var response = await part1.Content.ReadAsStringAsync();
            return (Result.Valid, response.Trim('"'));
        }



        private string ConstructLoginToken(Region region, string call)
        {
            var builder = new StringBuilder();
            builder.Append("https://");
            builder.Append(region switch
            {
                Region.NA => "usw",
                Region.LAN => "usw",
                Region.LAS => "usw",
                Region.OCE => "usw",
                Region.BR => "usw",
                Region.JP => "apne",
                _ => "euc"
            });
            builder.Append(".pp.riotgames.com");
            builder.Append(call);
            return builder.ToString();
        }
    }
}