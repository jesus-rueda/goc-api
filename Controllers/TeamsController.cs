using Goc.Api.Dtos;
using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController
    {
        private readonly GocContext context;

        public TeamsController(GocContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("{teamId}/Mission/{missionId}")]
        public async Task<ActionResult<TeamMission>> GetMissions(int teamId, int missionId)
        {
            var teamMission = new TeamMission()
            {
                MissionCompleteness = 80
            };

            return await Task.FromResult(teamMission);
        }
    }
}
