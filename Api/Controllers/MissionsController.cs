using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

using System;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Authorize]
[Route("/api/Campaigns/{campaignId}/[controller]")]
public class MissionsController: ControllerBase
{
    private readonly IMissionService myMissionService;

    public MissionsController(IMissionService missionService)
    {
        this.myMissionService = missionService;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<MissionsDto>> Get(
        [FromRoute] int id,
        [FromRoute] int campaignId)
    {
        var missions = await this.myMissionService.GetAsync(campaignId, id);
        return missions;
    }

    [HttpGet]
    public async Task<ActionResult<List<MissionsDto>>> GetCampaignMissions(int campaignId)
    {
        try
        {
            return await this.myMissionService.GetCampaignMissionsAsync(campaignId);
        }
        catch(Exception ex)
        {
            return this.Problem(ex.Message);
        }
        
    }
}