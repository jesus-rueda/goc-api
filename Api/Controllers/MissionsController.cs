using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers
{
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

        [HttpGet("All")]
        public async Task<ActionResult<List<MissionsDto>>> GetMissions()
        {
            var missions = await _missionBl.GetAllAsync();

            return missions;

            //var missiosn = new List<Missions>()
            //{
            //    new()
            //    {
            //        Id = 1, Name = "Mission 1",
            //        StartDate = DateTime.Now,
            //        EndDate = DateTime.Now.AddDays(30),
            //        Instructions = "Instructions mission 1",
            //        Coinks = 2500
            //    },

            //    new()
            //    {
            //        Id = 2, Name = "Mission 2",
            //        StartDate = DateTime.Now.AddMonths(1),
            //        EndDate = DateTime.Now.AddMonths(1),
            //        Instructions = "Instructions mission 2",
            //        Coinks = 5000
            //    }
            //};

            //return await Task.FromResult(missiosn);
        }
    }
}
