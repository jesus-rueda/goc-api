using System;
using System.IO;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Goc.Api.Controllers
{
    [ApiController]
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
        //public async Task<ActionResult<EvidencesDto>> Create([FromRoute] int missionId, ActionsLogDto action)
        {
            var action = new ActionsLog();
            if (missionId != action.MissionId)
            {
                return BadRequest();
            }

            var imageBase64 = "";
            if (formFile.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    imageBase64 = Convert.ToBase64String(fileBytes);
                }

                var evicence = await _evidenceBl.CreateAsync(action.MissionId, action.TeamId, action.ActionTypeId, action.TeamCharacterId, action.AffectedTeamId, imageBase64);

                return evicence;
            }

            return BadRequest();
        }
    }
}
