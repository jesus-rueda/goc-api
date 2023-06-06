using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Api.Dtos;
using Goc.Api.Extensions;
using Goc.Business.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamBl _teamBl;

        public TeamsController(ITeamBl teamsBl)
        {
            _teamBl = teamsBl;
        }

        [HttpGet]
        [Route("All")]
        public async Task<ActionResult<List<TeamsDto>>> GetAllTeams()
        {
            var teams = await _teamBl.GetAllAsync();

            return teams.ToDto();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TeamsDto>> Get(int id)
        {
            var team = await _teamBl.GetAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return team.ToDto();
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
