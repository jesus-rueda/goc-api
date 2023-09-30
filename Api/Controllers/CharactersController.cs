using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

using Microsoft.AspNetCore.Authorization;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ICharacterBl _characterBl;

    public CharactersController(ICharacterBl characterBl)
    {
        _characterBl = characterBl;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Characters>> Get(int id)
    {
        var characters = await _characterBl.Get(id);
        return characters == null ? NotFound() : characters;
    }


    //[HttpGet]
    //[Route("profile")]
    //public async Task<ActionResult<TeamCharacterProfileDto>> GetProfile()
    //{
    //    var userId = this.User.Identity.Name;
    //    var teamCharacter = await _characterBl.GetProfile(userId);
    //    return teamCharacter == null ? NotFound() : teamCharacter;
    //}
}