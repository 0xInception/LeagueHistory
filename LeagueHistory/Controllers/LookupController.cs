using System;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}