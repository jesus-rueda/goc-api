using System;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Models;
using Microsoft.EntityFrameworkCore;
using Goc.Business.Extensions;

namespace Goc.Business;

public class CharacterBl : ICharacterBl
{
    private readonly GocContext _context;

    public CharacterBl(GocContext context)
    {
        _context = context;
    }

    public async Task<Characters?> Get(int id)
    {
        var character = await this._context.Characters.FindAsync(id);
        return character;
    }

    //public async Task<TeamCharacterProfileDto> GetProfile(string email)
    //{
    //    var teamCharacterProfile = await _context.Users
    //        .Where(c => c.Upn == email)
    //        .Include(c => c.Team)
    //        .Include(c => c.Character)
    //        .FirstOrDefaultAsync();

    //    return teamCharacterProfile.ToDto();
    //}
}