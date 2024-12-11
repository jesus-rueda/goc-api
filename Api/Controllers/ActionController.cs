// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Api.Controllers;

using System.IO;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Authorize]
[Route("api/")]
public class ActionsController : ControllerBase
{
    #region Fields

    private readonly IActionsService myActionsService;

    private readonly IMissionService myMissionService;

    #endregion

    #region Constructors

    public ActionsController(IMissionService missionService, IActionsService actionsService)
    {
        this.myMissionService = missionService;
        this.myActionsService = actionsService;
    }

    #endregion

    #region Methods

    [HttpPost]
    [Route("campaigns/{campaignId}/[controller]/attack")]
    public async Task<ActionResult<GocActionResult>> Attack([FromRoute] int campaignId, [FromBody] AttackRequest attack)
    {
        var user = this.User.GetGocUser();
        var response = await this.myActionsService.Attack(campaignId, user, attack.TargetTeamId);
        return this.Ok(response);
    }

    [HttpPost]
    [Route("campaigns/{campaignId}/missions/{missionId}/[controller]/complete")]
    public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CompleteMission(
        [FromRoute] int campaignId,
        [FromRoute] int missionId,
        IFormFile formFile)
    {
        var user = this.User.GetGocUser();
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        var fileBytes = ms.ToArray();
        var result = await this.myActionsService.FinishMission(campaignId, missionId, user, fileBytes);
        return this.Ok(result);
    }

    [HttpPost]
    [Route("campaigns/{campaignId}/[controller]/setupDefense")]
    public async Task<Microsoft.AspNetCore.Mvc.ActionResult> SetupDefence([FromRoute] int campaignId, IFormFile formFile)
    {
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        var evidence = ms.ToArray();
        var user = this.User.GetGocUser();
        var response = await this.myActionsService.SetupDefence(campaignId, user, evidence);
        return this.Ok(response);
    }

    [HttpPost]
    [Route("Campaigns/{campaignId}/[controller]/defend")]
    public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Defend([FromRoute] int campaignId, int attackId, IFormFile formFile)
    {
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        var evidence = ms.ToArray();
        var user = this.User.GetGocUser();
        var response = await this.myActionsService.SetupDefence(campaignId, user, evidence);
        return this.Ok();
    }


    [HttpPost]
    [Route("Campaigns/{campaignId}/[controller]/duel")]
    public async Task<ActionResult<GocActionResult>> Duel([FromRoute] int campaignId, [FromBody] DuelRequests duel)
    {
        var user = this.User.GetGocUser();
        var response = await this.myActionsService.Duel(campaignId, user, duel.TargetTeamId, duel.GameId, duel.BetCoinks);
        return this.Ok(response);
    }

    #endregion

    //[HttpPost]
    //[Route("{campaignId}/{missionId}")]
    //public async Task<GocActionResult<EvidencesDto>> FinishMission(IFormFile formFile,
    //    [FromRoute] int missionId,
    //    [FromRoute] int? campaignId)
    //{
    //    var user = this.User.GetGocUser();

    //    var actionString = this.Request.Form["action"];
    //    var action = JsonConvert.DeserializeObject<ActionLogDto>(actionString);

    //    var imageBase64 = "";
    //    if (formFile is { Length: > 0 } && action.ActionTypeId != 2)
    //    {
    //        using var ms = new MemoryStream();
    //        await formFile.CopyToAsync(ms);
    //        var fileBytes = ms.ToArray();
    //        imageBase64 = Convert.ToBase64String(fileBytes);
    //    }

    //    try
    //    {
    //        if (campaignId == null)
    //        {
    //            var campaign = await this.myCampaignService.GetActive();
    //            campaignId = campaign.Id;
    //        }

    //        var evidence = await this.myEvidenceService.SendAsync(campaignId.Value, action.MissionId, action.TeamId, action.ActionTypeId, user.Id, action.AffectedTeamId, imageBase64);
    //        return evidence;
    //    }
    //    catch (Exception exc)
    //    {
    //        return BadRequest(exc.Message);
    //    }
    //}
}