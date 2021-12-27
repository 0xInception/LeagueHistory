using System;
using System.Text.Json;

namespace LeagueHistory.Core
{
    public class AccessToken
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? session_state { get; set; }
        public string? id_token { get; set; }
        public string? scope { get; set; }

        public AccessToken(string? response)
        {
            var json = JsonDocument.Parse(response!);
            var uri = json.RootElement.GetProperty("response").GetProperty("parameters").GetProperty("uri").GetString();
            var secondPart = uri?.Split('#')[1];
            var results = secondPart?.Split('&');
            foreach (var res in results!)
            {
                string?[] split = res.Split('=');
                switch (split[0])
                {
                    case "access_token":
                        access_token = split[1];
                        break;
                    case "scope":
                        scope = split[1];
                        break;
                    case "id_token":
                        id_token = split[1];
                        break;
                    case "token_type":
                        token_type = split[1];
                        break;
                    case "expires_in":
                        expires_in = Int32.Parse(split[1]!);
                        break;
                    case "session_state":
                        session_state = split[1];
                        break;
                    default:
                        // TODO: Notify
                        break;
                }
            }
        }
    }
}