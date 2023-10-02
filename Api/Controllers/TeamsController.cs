using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Newtonsoft.Json;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamBl _teamBl;

    public TeamsController(ITeamBl teamsBl)
    {
        _teamBl = teamsBl;
    }




    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<TeamDto>>> GetAllTeams()
    {
        var teams = await _teamBl.GetAllAsync();
        return teams;
    }

    [HttpGet]
    [Route("{teamId}")]
    public async Task<ActionResult<TeamDto>> Get(int teamId)
    {
        var team = await _teamBl.GetAsync(teamId);
        if (team == null)
        {
            return NotFound();
        }

        return team;
    }

    [HttpPost]
    [Route("{teamId}/joins")]
    public async Task<ActionResult<JoinRequestsResponse>> Join([FromRoute] int teamId, [FromBody] JoinRequets request)
    {
        var user = this.User.GetGocUser();
        await this._teamBl.RequestJoin(user.Id, teamId, request.CharacterId);
        return new JoinRequestsResponse { Placed = true };
    }

    [HttpGet]
    [Authorize(Roles = "leader")]
    [Route("{teamId}/joins")]
    public async Task<ActionResult<ResultCollection<UserProfile>>> GetJoinRequests(int teamId)
    {
        var user = this.User.GetGocUser();
        if (user.TeamId != teamId)
        {
            return this.Forbid();
        }

        var users = await this._teamBl.GetPendingJoinAprovals(teamId);
        return this.Ok(users.ToResultCollection());
    }

    [HttpPost]
    [Authorize(Roles = "leader")]
    [Route("{teamId}/joins/aprovals")]
    public async Task<ActionResult> Aprove(int teamId, AproveJoinRequests request)
    {
        var user = this.User.GetGocUser();
        if (user.TeamId != teamId)
        {
            return this.Forbid();
        }

        await this._teamBl.AproveJoinRequest(teamId, request.UserId, request.Aprove);
        return this.NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [Route("")]
    public async Task<ActionResult<TeamDto>> Create(IFormFile formFile)
    {
        var requestStr = this.Request.Form["request"];
        var request = JsonConvert.DeserializeObject<TeamCreationRequests>(requestStr);

        var bytes = Array.Empty<byte>();
        if (formFile != null && formFile.Length > 0)
        {
            using var ms = new MemoryStream();
            formFile.CopyTo(ms);
            bytes = ms.ToArray();

        }
        var team = await this._teamBl.Create(request.Name, bytes, request.LeaderId);
        return this.Ok(team);
    }



    [HttpPost]
    [Authorize(Roles = "admin")]
    [Route("{teamId}/leader/")]
    public async Task<ActionResult> MakeLeader(int teamId, LeaderRequests requests)
    {        
        await this._teamBl.MakeLeader(teamId, requests.userId);
        return this.Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{teamId}/image")]
    public async Task GetImage([FromRoute] int teamId)
    {
        var image = await _teamBl.GetImage(teamId); // replace with actual method to get image
        
        if (image != null)
        {
            this.Response.ContentType = "image/png";
            await this.Response.Body.WriteAsync(image);
        }
        else
        {
            this.Response.StatusCode = 404;
        }
    }





    [HttpGet]
    [Route("members/{memberId}/stats")]
    public async Task<ActionResult<TeamMemberStats>> GetTeamMember(int memberId)
    {
        return await this._teamBl.GetTeamMemberStatsAsync(memberId);
    }



    [HttpDelete]
    [Route("{teamId}")]
    [Authorize(Roles = "admin")]
    public async Task Delete(int teamId)
    {
        await this._teamBl.Delete(teamId);
    }


    [HttpGet]
    [Route("{teamId}/Mission/{missionId}")]
    public async Task<ActionResult<TeamMission>> GetMission(int teamId, int missionId)
    {
        return await this._teamBl.GetMissionProgressAsync(missionId, teamId);
    }
}
