using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

using System.Linq;
using Goc.Business.Dtos;

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

    public async Task<TeamMission> GetMissionProgressAsync(int missionId, int teamId)
    {
        var total = await this._context.TeamsCharacters.CountAsync(c => c.TeamId == teamId);

        var actionsCount = await this._context.ActionsLog
            .Include(a => a.Evidences)
            .Where(a => a.TeamId == teamId && a.MissionId == missionId && a.Evidences.FirstOrDefault().IsValid)
            .GroupBy(a => a.TeamCharacterId)
            .CountAsync();

        var progress = (100 * actionsCount) / total;

        return new TeamMission { MissionCompleteness = progress };
    }

    public async Task<List<Teams>> GetAllAsync()
    {
        var teams = await _context.Teams.ToListAsync();

        return teams;
    }
}
