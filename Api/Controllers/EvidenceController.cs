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
        private readonly IEvidenceBl _evidenceBl;

        public EvidenceController(IEvidenceBl evidenceBl)
        {
            _evidenceBl = evidenceBl;
        }


        [HttpPost]
        [Route("{missionId}")]
        public async Task<ActionResult<EvidencesDto>> Create(IFormFile formFile, [FromRoute] int missionId)
        {
            var actionString = this.Request.Form["action"];
            var action = JsonConvert.DeserializeObject<ActionsLogDto>(actionString);

            var imageBase64 = "";
            if (formFile != null && formFile.Length > 0 && action.ActionTypeId != 2)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    imageBase64 = Convert.ToBase64String(fileBytes);
                }
            }

            try
            {
                var evidence = await _evidenceBl.CreateAsync(action.MissionId, action.TeamId, action.ActionTypeId, action.TeamCharacterId, action.AffectedTeamId, imageBase64);
                return evidence;
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}
