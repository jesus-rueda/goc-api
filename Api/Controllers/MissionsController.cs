using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionsController
{
    private readonly IMissionBl _missionBl;

    public MissionsController(IMissionBl missionBl)
    {
        _missionBl = missionBl;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<MissionsDto>> Get(int id)
    {
        var missions = await _missionBl.GetAsync(id);

        return missions;
    }

    [HttpGet("/api/Campaigns/{campaignId}/Missions")]
    public async Task<ActionResult<List<MissionsDto>>> GetCampaignMissions(int campaignId)
    {
        return await _missionBl.GetCampaignMissionsAsync(campaignId);
    }
}