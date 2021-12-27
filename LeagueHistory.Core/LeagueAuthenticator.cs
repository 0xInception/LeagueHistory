using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using LeagueHistory.Core.Architecture;

namespace LeagueHistory.Core
{
    // Why did i decide to switch to httpclient :facepalm:
    public class LeagueAuthenticator : ILeagueAuthenticator
    {
        private const string
            AUTH_URL =
                "https://auth.riotgames.com/api/v1/authorization"; // Let's use the new api in case they deprecate the old one.

        public HttpClient AuthenticatorClient { get; set; }
        private Random _random;

        public LeagueAuthenticator()
        {
            AuthenticatorClient = new HttpClient();
            _random = new Random();
        }

        public async Task<Result> Authenticate(LeagueAccount account)
        {
            var part1 = await AuthenticatorClient.PostAsync(AUTH_URL,
                new StringContent(GeneratePart1(), Encoding.UTF8, "application/json"));
            //TODO: Extract the cookies and use them on the following PUT request.

            //TODO: Part2 PUT {"language": "en_GB","password": "","region": None,"remember": False,"type": "auth","username": ""}
            return Result.Unknown;

            string GeneratePart1() // TODO: Maybe extract this - StringBuilder implementation is fine tbh, let's avoid unnecessary serialization.
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
        }

        public async Task<Result>
            Refresh(LeagueAccount account) // TODO: Find a way of refreshing the token. LOOK /userinfo
        {
            throw new NotImplementedException();
        }
    }
}