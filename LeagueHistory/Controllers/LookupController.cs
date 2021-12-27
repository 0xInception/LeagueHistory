using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueHistory.Core;
using LeagueHistory.Core.Enums;
using LeagueHistory.Core.Interfaces;
using LeagueHistory.Core.JsonObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueHistory.Controllers
{
    [ApiController]
    [Route("/Lookup")]
    public class LookupController : Controller
    {
        public IServiceProvider ServiceProvider { get; }

        public LookupController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Lookup([FromQuery(Name = "Region")] string region,
            [FromQuery(Name = "summonerName")] string summonerName, [FromQuery(Name = "puuid")] string puuid)
        {
            if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(summonerName) && string.IsNullOrEmpty(puuid))
            {
                return BadRequest("F");
            }

            var leagueApi = ServiceProvider.GetService<ILeagueApi>();
            if (!string.IsNullOrEmpty(region) && !string.IsNullOrEmpty(summonerName))
            {
                // TODO: Make it so it preserves this response?
                var summoner = await leagueApi.Lookup(summonerName, Enum.Parse<Region>(region, true));
                return await Lookup(string.Empty, string.Empty, summoner.puuid);
            }
            else if (!string.IsNullOrEmpty(puuid))
            {
                var responses = await GetResponses();
                return Ok(JsonSerializer.Serialize(responses,new JsonSerializerOptions(JsonSerializerDefaults.Web)));
                // TODO: Models
            }

            return View();

            async Task<List<LookupResponse>> GetResponses()
            {
                var ret = new List<LookupResponse>();
                foreach (var r in Enum.GetValues<Region>())
                {
                    ret.Add(await leagueApi.LookupPuuid(puuid, r));
                }

                return ret;
            }
        }
    }
}