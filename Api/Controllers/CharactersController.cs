using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using System.Collections.ObjectModel;
using System.Linq;

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
    public async Task<ActionResult<Character>> Get(int id)
    {
        var characters = await _characterBl.Get(id);
        return characters == null ? NotFound() : characters;
    }
    [HttpGet]    
    public async Task<ActionResult<ResultCollection<Character>>> Get()
    {
        var characters = await _characterBl.GetAll();
        return characters == null ? NotFound() : characters.ToResultCollection();
    }


    [HttpGet]
    [Route("image/{characterId}")]
    public IActionResult GetImage(int characterId, [FromQuery] string type)
    {
        var image = _characterBl.GetImage(characterId, type); // replace with actual method to get image
        if (image != null)
        {
            return File(image, "image/jpeg"); // replace with actual image file type
        }
        else
        {
            return NotFound();
        }
    }



    //[HttpGet]
    //[Route("profile")]
    //public async Task<GocActionResult<TeamCharacterProfileDto>> GetProfile()
    //{
    //    var userId = this.User.Identity.Name;
    //    var teamCharacter = await _characterBl.GetProfile(userId);
    //    return teamCharacter == null ? NotFound() : teamCharacter;
    //}
}