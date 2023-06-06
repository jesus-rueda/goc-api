using Goc.Api.Dtos;
using Goc.Business.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamBl _temaBl;

        public TeamsController(ITeamBl teamsBl)
        {
            _temaBl = teamsBl;
        }


        [HttpGet]
        [Route("All")]
        public async Task<ActionResult<List<Models.Teams>>> GetAllTeams()
        {
            var teams = await _temaBl.GetAll();

            return teams;
        }



        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Models.Teams>> Get(int id)
        {
            var teams = await _temaBl.Get(id);
            if (teams == null)
            {
                return NotFound();
            }

            return teams;
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
