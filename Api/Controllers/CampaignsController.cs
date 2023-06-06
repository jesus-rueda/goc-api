using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignBl _campaignBl;

    public CampaignsController(ICampaignBl campaignBl)
    {
        _campaignBl = campaignBl;
    }

    [HttpGet]
    [Route("All")]
    public async Task<ActionResult<List<Campaigns>>> GetAll()
    {
        var campaigns = await _campaignBl.GetAll();
        return campaigns;
    }
}