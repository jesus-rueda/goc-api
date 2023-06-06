using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController
    {
        private readonly GocContext context;

        public MissionsController(GocContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Missions>>> GetMissions()
        {
            var missiosn = new List<Missions>()
            {
                new()
                {
                    Id = 1, Name = "Mission 1",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    Instructions = "Instructions mission 1",
                    Coinks = 2500
                },

                new()
                {
                    Id = 2, Name = "Mission 2",
                    StartDate = DateTime.Now.AddMonths(1),
                    EndDate = DateTime.Now.AddMonths(1),
                    Instructions = "Instructions mission 2",
                    Coinks = 5000
                }
            };

            return await Task.FromResult(missiosn);
        }
    }
}
