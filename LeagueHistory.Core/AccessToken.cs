using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using LeagueHistory.Core.Enums;

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
        public Region Region { get; set; }

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
                        var payload = id_token.Split('.')[1];
                        int padding = payload.Length % 4;
                        if (padding > 0 )
                        {
                            payload += new string('=', 4 - padding);
                        }
                        var json2 = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
                        var regionString = json2.RootElement.GetProperty("lol_region").EnumerateArray().First();
                        var pid = regionString.GetProperty("pid").GetString();
                        Region = Enum.Parse<Platform>(pid, true).ToRegion();
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
                }
            }
        }
    }
}