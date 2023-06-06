using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class MissionBl : IMissionBl
{
    private readonly GocContext _context;

    public MissionBl(GocContext context)
    {
        this._context = context;
    }

    public async Task<Missions> GetAsync(int id)
    {
        var mission = await _context.Missions.FindAsync(id);

        return mission;
    }

    public async Task<List<Missions>> GetAllAsync()
    {
        var missions = await _context.Missions.ToListAsync();

        return missions;
    }
}

