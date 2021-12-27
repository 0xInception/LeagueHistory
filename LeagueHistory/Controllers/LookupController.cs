using System;
using System.Collections.Generic;
using System.Diagnostics;
using LeagueHistory.Core;
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
        public IActionResult Lookup([FromQuery(Name = "Region")] string region,
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
                var summoner = leagueApi.Lookup(summonerName, Enum.Parse<Region>(region, true));
                return Lookup(string.Empty, string.Empty, summoner.puuid);
            }
            else if (!string.IsNullOrEmpty(puuid))
            {
                var responses = GetResponses();
                // TODO: Models
            }

            return View();

            IEnumerable<LookupResponse> GetResponses()
            {
                foreach (var r in Enum.GetValues<Region>())
                {
                    yield return leagueApi.LookupPuuid(puuid, r);
                }
            }
        }
    }
}