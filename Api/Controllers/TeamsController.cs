// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Api.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

[ApiController]
[Authorize]
[Route("api/")]
public class TeamsController : ControllerBase
{
    #region Fields

    private readonly ICampaignService myCampaignService;

    private readonly ITeamService myTeamService;

    #endregion

    #region Constructors

    public TeamsController(ITeamService teamsService, ICampaignService campaign)
    {
        this.myTeamService = teamsService;
        this.myCampaignService = campaign;
    }

    #endregion

    #region Methods

    [HttpPost]
    [Authorize(Roles = "leader")]
    [Route("[controller]/{teamId}/joins/approvals")]
    public async Task<ActionResult> Approve(int teamId, AproveJoinRequests request)
    {
        return await this.Approve(null, teamId, request);
    }

    [HttpPost]
    [Authorize(Roles = "leader")]
    [Route("Campaigns/{campaignId}/[controller]/{teamId}/joins/approvals")]
    public async Task<ActionResult> Approve(
        [FromRoute] int? campaignId,
        [FromRoute] int teamId, AproveJoinRequests request)
    {
        var user = this.User.GetGocUser();
        if (user.TeamId != teamId)
        {
            return this.Forbid();
        }
        if(campaignId == null)
        {
            var campaign = await this.myCampaignService.GetActive();
            campaignId = campaign.Id;
        }
        
        await this.myTeamService.ApproveJoinRequest(campaignId.Value, teamId, request.UserId, request.Aprove);
        return this.NoContent();
    }





    [HttpPost]
    [Authorize(Roles = "admin")]
    [Route("[controller]")]
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

        var team = await this.myTeamService.Create(request.Name, bytes, request.LeaderId);
        return this.Ok(team);
    }

    [HttpDelete]
    [Route("[controller]/{teamId}")]
    [Authorize(Roles = "admin")]
    public async Task Delete(int teamId)
    {
        await this.myTeamService.Delete(teamId);
    }

    [HttpGet]
    [Route("[controller]/{teamId}")]
    public async Task<ActionResult<TeamDto>> Get(int teamId)
    {
        var campaignId = await this.myCampaignService.GetActive();
        var team = await this.myTeamService.GetAsync(campaignId.Id, teamId);
        if (team == null)
        {
            return this.NotFound();
        }

        return team;
    }

    [HttpGet]
    [Route("[controller]")]
    public async Task<ActionResult<List<TeamDto>>> GetAllTeams()
    {
        var campaign = await this.myCampaignService.GetActive();
        var teams = await this.myTeamService.GetAllAsync(campaign.Id);
        return teams;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("[controller]/{teamId}/image")]
    public async Task GetImage([FromRoute] int teamId)
    {
        var image = await this.myTeamService.GetImage(teamId); // replace with actual method to get image

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
    [Authorize(Roles = "leader")]
    [Route("campaigns/{campaignId}/[controller]/{teamId}/joins")]
    public async Task<ActionResult<ResultCollection<UserProfile>>> GetJoinRequests(
        [FromRoute] int? campaignId,
        [FromRoute] int teamId)
    {
        var user = this.User.GetGocUser();
        if (user.TeamId != teamId)
        {
            return this.Forbid();
        }
        if(campaignId == null)
        {
            var campaign = await this.myCampaignService.GetActive();
            campaignId = campaign.Id;
        }

        var users = await this.myTeamService.GetPendingJoinApprovals(campaignId.Value, teamId);
        return this.Ok(users.ToResultCollection());
    }

    [HttpGet]
    [Authorize(Roles = "leader")]
    [Route("[controller]/{teamId}/joins")]
    public async Task<ActionResult<ResultCollection<UserProfile>>> GetJoinRequests(int teamId)
    {
        return await this.GetJoinRequests(null, teamId);
    }

    [HttpGet]
    [Route("[controller]/{teamId}/Mission/{missionId}")]
    public async Task<ActionResult<TeamMission>> GetMissionProgress(int teamId, int missionId)
    {
        return await this.GetMissionProgress(null, missionId, teamId);
    }


    [HttpGet]
    [Route("Campaigns/{campaignId}/[controller]/{teamId}/Mission/{missionId}")]
    public async Task<ActionResult<TeamMission>> GetMissionProgress(
        [FromRoute] int? campaignId, 
        [FromRoute] int teamId,
        [FromRoute] int missionId)
    {
        if (campaignId == null)
        {
            var campaign = await this.myCampaignService.GetActive();
            campaignId = campaign.Id;
        }

        return await this.myTeamService.GetMissionProgressAsync(campaignId.Value, missionId, teamId);
    }

    [HttpGet]
    [Route("[controller]/members/{userId}/stats")]
    public async Task<ActionResult<TeamMemberStats>> GetTeamMemberStats(int userId)
    {
        return await this.GetTeamMemberStats(null , userId);
    }

    [HttpGet]
    [Route("Campaigns/{campaignId}/[controller]/members/{userId}/stats")]
    public async Task<ActionResult<TeamMemberStats>> GetTeamMemberStats(
        [FromRoute] int? campaignId,
        [FromRoute] int userId
        )
    {
        if (campaignId == null)
        {
            var campaign = await this.myCampaignService.GetActive();
            campaignId = campaign.Id;
        }

        return await this.myTeamService.GetTeamMemberStatsAsync(campaignId.Value, userId);
    }

    [HttpPost]
    [Route("[controller]/{teamId}/joins")]
    public async Task<ActionResult<JoinRequestsResponse>> Join([FromRoute] int teamId, [FromBody] JoinRequets request)
    {
        return await this.Join(null, teamId, request);
    }

    [HttpPost]
    [Route("Campaigns/{campaignId}/[controller]/{teamId}/joins")]
    public async Task<ActionResult<JoinRequestsResponse>> Join(
        [FromRoute] int? campaignId,
        [FromRoute] int teamId, 
        [FromBody] JoinRequets request)
    {
        if(campaignId == null)
        {
            var campaign = await this.myCampaignService.GetActive();
            campaignId = campaign.Id;
        }
        
        var user = this.User.GetGocUser();
        await this.myTeamService.RequestJoin(campaignId.Value, user.Id, teamId, request.CharacterId);
        return new JoinRequestsResponse { Placed = true };
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [Route("[controller]/{teamId}/leader/")]
    public async Task<ActionResult> MakeLeader(
        [FromRoute] int teamId,
        [FromBody] LeaderRequests requests)
    {
        return await this.MakeLeader(null, teamId, requests);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [Route("Campaigns/{campaignId}/[controller]/{teamId}/leader/")]
    public async Task<ActionResult> MakeLeader(
        [FromRoute] int? campaignId,
        [FromRoute] int teamId,
        [FromBody] LeaderRequests requests)
    {
        if(campaignId == null)
        {
            var campaign = await this.myCampaignService.GetActive();
            campaignId = campaign.Id;
        }
        
        await this.myTeamService.MakeLeader(campaignId.Value, teamId, requests.userId);
        return this.Ok();
    }

    #endregion
}