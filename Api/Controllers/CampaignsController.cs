using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

using Microsoft.AspNetCore.Authorization;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService myCampaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        this.myCampaignService = campaignService;
    }

    [HttpGet]
    [Route("Active")]
    public async Task<ActionResult<Campaign>> GetActive()
    {
        return await this.myCampaignService.GetActive();
    }
}