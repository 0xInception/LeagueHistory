using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{
    // Why did i decide to switch to httpclient :facepalm:
    public class LeagueAuthenticator : ILeagueAuthenticator
    {
        private const string
            AUTH_URL =
                "https://auth.riotgames.com/api/v1/authorization"; // Let's use the new api in case they deprecate the old one.

        private readonly Random _random;
        public HttpClient AuthenticatorClient { get; set; }

        public LeagueAuthenticator()
        {
            AuthenticatorClient = new HttpClient();
            _random = new Random();
        }

        public async Task<Result> Authenticate(LeagueAccount account)
        {
            var part1 = await AuthenticatorClient.PostAsync(AUTH_URL,
                new StringContent(GeneratePart1(), Encoding.UTF8, "application/json"));
            if (!part1.IsSuccessStatusCode)
                return Result.Unknown;

            // I hate this but whatever we can refactor later
            var cookies = string.Join(' ', part1.Headers.GetValues("Set-Cookie").Select(d => d.Split(';')[0] + ";"));
            var request = new HttpRequestMessage(HttpMethod.Put, AUTH_URL);
            request.Headers.Add("Cookie", cookies);
            request.Content = new StringContent(GeneratePart2(), Encoding.UTF8, "application/json");
            var part2 = await AuthenticatorClient.SendAsync(request);
            if (!part2.IsSuccessStatusCode)
                return Result.Unknown;
            var response = await part2.Content.ReadAsStringAsync();
            switch (response)
            {
                case { } when response.Contains("auth_failure"):
                    return Result.Invalid;
                case { } when response.Contains("limit"):
                    return Result.RateLimited;
                case { } when !response.Contains("access_token"):
                    return Result.Unknown;
            }
            account.AccessToken = new AccessToken(response);
            return Result.Valid;

            string
                GeneratePart1() // TODO: Maybe extract this - StringBuilder implementation is fine tbh, let's avoid unnecessary serialization.
            {
                var stringBuilder = new StringBuilder(byte.MaxValue); // Won't be longer than 0xFF
                stringBuilder.Append(
                    "{\"acr_values\": \"\",\"claims\": \"\",\"client_id\": \"riot-client\",\"nonce\":\"");
                stringBuilder.Append(RandomString(22));
                stringBuilder.Append(
                    "\",\"code_challenge\": \"\",\"code_challenge_method\": \"\",\"redirect_uri\": \"http://localhost/redirect\",\"response_type\": \"token id_token\",\"scope\": \"openid link ban lol_region\"}");
                return stringBuilder.ToString();

                string RandomString(int length) // TODO: Random provider
                    =>
                        new(Enumerable
                            .Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_", length)
                            .Select(s => s[_random.Next(s.Length)]).ToArray());
            }

            string GeneratePart2()
            {
                var stringBuilder = new StringBuilder(byte.MaxValue);
                stringBuilder.Append(
                    "{\"language\": \"en_GB\",\"password\": \"");
                stringBuilder.Append(account.Credentials.Password);
                stringBuilder.Append(
                    "\",\"region\": null,\"remember\": false,\"type\": \"auth\",\"username\": \"");
                stringBuilder.Append(account.Credentials.Username);
                stringBuilder.Append("\"}");
                return stringBuilder.ToString();
            }
        }

        public async Task<Result>
            Refresh(LeagueAccount account) // TODO: Find a way of refreshing the token. LOOK /userinfo
        {
            throw new NotImplementedException();
        }
    }
}