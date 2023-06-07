using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

using System;
using System.Linq;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;

public class TeamBl : ITeamBl
{
    private readonly GocContext _context;

    public TeamBl(GocContext context)
    {
        _context = context;
    }

    public async Task<TeamsDto> GetAsync(int id)
    {
        var team = await _context.Teams
            .Include(x => x.TeamsCharacters)
            .ThenInclude(x=>x.Character)
            .FirstAsync(x => x.Id == id);

        var dto = team.ToDto();

        var teamMembersCount = await this._context.TeamsCharacters.CountAsync(x => x.TeamId == id);
        dto.AttacksDone = await this._context.ActionsLog.CountAsync(x=>x.TeamId == id && x.ActionTypeId == 2);
        dto.AttacksTotal = teamMembersCount * 5;
        dto.DefensesTotal = teamMembersCount * 5;
        dto.DefensesUsed = await _context.ActionsLog.CountAsync(x => x.TeamId == id && x.ActionTypeId == 3);

        return dto;
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

    public async Task<TeamMemberStats> GetTeamMemberStatsAsync(int memberId)
    {

        var attacks = await this._context.ActionsLog.CountAsync(a => a.TeamCharacterId == memberId && a.ActionTypeId == 2);
        var defends = await this._context.ActionsLog.CountAsync(a => a.TeamCharacterId == memberId && a.ActionTypeId == 3);
        

        return new TeamMemberStats
               {
                   AttacksDone = attacks,
                   AttacksTotal = 5,
                   DefensesTotal = 5,
                   DefensesUsed = defends
               };
    }

    public async Task<List<Teams>> GetAllAsync()
    {
        var teams = await _context.Teams.ToListAsync();
        return teams;
    }
}
