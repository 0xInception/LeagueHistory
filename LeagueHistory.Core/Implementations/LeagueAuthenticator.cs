using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueHistory.Core.Enums;
using LeagueHistory.Core.Interfaces;
using Microsoft.VisualBasic;

namespace LeagueHistory.Core.Implementations
{
    // Why did i decide to switch to httpclient :facepalm:
    public class LeagueAuthenticator : ILeagueAuthenticator
    {

        private const string
            AUTH_URL =
                "https://auth.riotgames.com/api/v1/authorization"; // Let's use the new api in case they deprecate the old one.
        private const string USERINFO = "https://auth.riotgames.com/userinfo";
        private const string ENTITLEMENTS = "https://entitlements.auth.riotgames.com/api/token/v1";

        public LeagueAuthenticator(IRandomProvider randomProvider)
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
            var cookies = await FetchCookies();
            if (cookies.result != Result.Valid)
                return cookies.result;
            
            var auth = await Authenticate(account.Credentials,cookies.cookies);
            if (auth.result != Result.Valid)
                return auth.result;
            account.Info.AccessToken = auth.accessToken;
            var userInfo = await UserInfo(account.Info.AccessToken);
            if (userInfo.result != Result.Valid)
                return auth.result;
            var entitlements = await Entitlements(account.Info.AccessToken);
            if (entitlements.result != Result.Valid)
                return auth.result;

            account.Info.UserInfo = userInfo.userInfo;
            account.Info.Entitlements = entitlements.entitlements;
            return Result.Valid;
           
        }

        private async Task<(Result result, string cookies)> FetchCookies()
        {
            var stringBuilder = new StringBuilder(byte.MaxValue); // Won't be longer than 0xFF
            stringBuilder.Append(
                "{\"acr_values\": \"\",\"claims\": \"\",\"client_id\": \"lol\",\"nonce\":\"");
            stringBuilder.Append(RandomProvider.RandomString(22));
            stringBuilder.Append(
                "\",\"code_challenge\": \"\",\"code_challenge_method\": \"\",\"redirect_uri\": \"http://localhost/redirect\",\"response_type\": \"token id_token\",\"scope\": \"openid link ban lol_region\"}");
            
            var part1 = await AuthenticatorClient.PostAsync(AUTH_URL,
                new StringContent(stringBuilder.ToString(), Encoding.UTF8, "application/json"));
            if (!part1.IsSuccessStatusCode)
                return (Result.Unknown,string.Empty);

            var setCookie = part1.Headers.GetValues("Set-Cookie").ToArray();
            if (setCookie.Length < 3)
                return (Result.RateLimited, string.Empty);
            
            var cookies = string.Join(' ', setCookie.Select(d => d.Split(';')[0] + ";"));
            return (Result.Valid, cookies);
        }

        private async Task<(Result result, AccessToken accessToken)> Authenticate(LeagueCredentials credentials,string cookies)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, AUTH_URL);
            request.Headers.Add("Cookie", cookies);
            var stringBuilder = new StringBuilder(byte.MaxValue);
            stringBuilder.Append(
                "{\"language\": \"en_GB\",\"password\": \"");
            stringBuilder.Append(credentials.Password);
            stringBuilder.Append(
                "\",\"region\": null,\"remember\": false,\"type\": \"auth\",\"username\": \"");
            stringBuilder.Append(credentials.Username);
            stringBuilder.Append("\"}");
            request.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "application/json");
            var part2 = await AuthenticatorClient.SendAsync(request);
            if (!part2.IsSuccessStatusCode)
                return (Result.Unknown,null)!;
            var response = await part2.Content.ReadAsStringAsync();
            switch (response)
            {
                case { } when response.Contains("auth_failure"):
                    return (Result.Invalid,null)!;
                case { } when response.Contains("limit"):
                    return (Result.RateLimited,null)!;
                case { } when !response.Contains("access_token"): 
                    return (Result.Unknown,null)!;
            }
            return (Result.Valid,new AccessToken(response));
        }

        private async Task<(Result result, string userInfo)> UserInfo(AccessToken token) 
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, USERINFO);
            requestMessage.Headers.Add("Authorization",$"{token.token_type} {token.access_token}");
            requestMessage.Headers.Add("Accept","application/json");
            requestMessage.Headers.Add("User-Agent", "LeagueOfLegendsClient/12.1.416.5961");
            var result = await AuthenticatorClient.SendAsync(requestMessage);
            if (!result.IsSuccessStatusCode)
                return (Result.Unknown,string.Empty);
            var response = await result.Content.ReadAsStringAsync();
            return (Result.Valid, response);
        }
        
        private async Task<(Result result, string entitlements)> Entitlements(AccessToken token) 
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ENTITLEMENTS);
            requestMessage.Headers.Add("Authorization",$"{token.token_type} {token.access_token}");
            requestMessage.Headers.Add("Accept","application/json");
            requestMessage.Headers.Add("User-Agent", "LeagueOfLegendsClient/12.1.416.5961");
            requestMessage.Content =
                new StringContent("{\"urn\":\"urn:entitlement:%\"}", Encoding.UTF8, "application/json");
            var result = await AuthenticatorClient.SendAsync(requestMessage);
            if (!result.IsSuccessStatusCode)
                return (Result.Unknown,string.Empty);
            var response = await result.Content.ReadAsStringAsync();
            if (!response.Contains("entitlements"))
                return (Result.Unknown, string.Empty);
            var json = JsonDocument.Parse(response);
            return (Result.Valid, json.RootElement.GetProperty("entitlements_token").GetString())!;
        }
        
        public async Task<Result>
            Refresh(LeagueAccount account) // TODO: Find a way of refreshing the token. LOOK /userinfo (or old way will do for now)
        {
            throw new NotImplementedException();
        }
    }
}