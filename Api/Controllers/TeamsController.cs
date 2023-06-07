using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

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

        return team;
    }



    [HttpGet]
    [Route("/members/{memberId}/stats")]
    public async Task<ActionResult<TeamMemberStats>> GetTeamMember(int memberId)
    {
        return await this._teamBl.GetTeamMemberStatsAsync(memberId);
    }





    [HttpGet]
    [Route("{teamId}/Mission/{missionId}")]
    public async Task<ActionResult<TeamMission>> GetMission(int teamId, int missionId)
    {
        return await this._teamBl.GetMissionProgressAsync(missionId, teamId);
    }
}
