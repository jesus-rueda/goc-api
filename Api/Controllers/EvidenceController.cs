using System;
using System.IO;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Goc.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceService myEvidenceService;

        private readonly ICampaignService myCampaignService;

        public EvidenceController(IEvidenceService evidenceService, ICampaignService campaignService)
        {
            this.myEvidenceService = evidenceService;
            this.myCampaignService = campaignService;
        }

        [HttpPost]
        [Route("{missionId}")]
        
        public async Task<ActionResult<EvidencesDto>> Create(IFormFile formFile, [FromRoute] int missionId)
        {
            return await this.Create(formFile, missionId, null);
        }


        [HttpPost]
        [Route("{campaignId}/{missionId}")]
        public async Task<ActionResult<EvidencesDto>> Create(IFormFile formFile,
            [FromRoute] int missionId,
            [FromRoute] int? campaignId)
        {
            var user = this.User.GetGocUser();

            var actionString = this.Request.Form["action"];
            var action = JsonConvert.DeserializeObject<ActionLogDto>(actionString);

            var imageBase64 = "";
            if (formFile is { Length: > 0 } && action.ActionTypeId != 2)
            {
                using var ms = new MemoryStream();
                await formFile.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                imageBase64 = Convert.ToBase64String(fileBytes);
            }

            try
            {
                if (campaignId == null)
                {
                    var campaign = await this.myCampaignService.GetActive();
                    campaignId = campaign.Id;
                }

                var evidence = await this.myEvidenceService.CreateAsync(campaignId.Value, action.MissionId, action.TeamId, action.ActionTypeId, user.Id, action.AffectedTeamId, imageBase64);
                return evidence;
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}
