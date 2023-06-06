using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class TeamBl : ITeamBl
{
    private readonly GocContext _context;

    public TeamBl(GocContext context)
    {
        this._context = context;
    }

    public async Task<Teams?> GetAsync(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        return team;
    }

    public async Task<List<Teams>> GetAllAsync()
    {
        var teams = await _context.Teams.ToListAsync();

        return teams;
    }
}
