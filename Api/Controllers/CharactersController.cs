using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

[ApiController]
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
}