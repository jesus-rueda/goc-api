using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

using System.Linq;

public class TeamBl : ITeamBl
{
    private readonly GocContext _context;

    public TeamBl(GocContext context)
    {
        _context = context;
    }

    public async Task<Teams> GetAsync(int id)
    {
        var team = await _context.Teams
            .Include(x => x.TeamsCharacters)
            .ThenInclude(x=>x.Character)
            .FirstAsync(x => x.Id == id);

        return team;
    }

    public async Task<List<Teams>> GetAllAsync()
    {
        var teams = await _context.Teams.ToListAsync();

        return teams;
    }
}
