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


public class TeamBl : ITeamBl
{
    private readonly GocContext _context;

    public TeamBl(GocContext context)
    {
        _context = context;
    }

    public async Task<TeamDto> GetAsync(int id)
    {
        var team = await _context.Teams
            .Include(x => x.TeamsCharacters)
            .ThenInclude(x=>x.Character)
            .FirstAsync(x => x.Id == id);

        var dto = team.ToDto();

        var teamMembersCount = await this._context.Users.CountAsync(x => x.TeamId == id && !x.PendingAproval);

        dto.AttacksDone = await this._context.ActionsLog.CountAsync(x=>x.TeamId == id && x.ActionTypeId == 2);
        dto.AttacksTotal = teamMembersCount * 5;
        dto.DefensesTotal = teamMembersCount * 5;
        dto.DefensesUsed = await _context.ActionsLog.CountAsync(x => x.TeamId == id && x.ActionTypeId == 3);

        return dto;
    }

    public async Task<TeamMission> GetMissionProgressAsync(int missionId, int teamId)
    {
        var total = await this._context.Users.CountAsync(c => c.TeamId == teamId);

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

    public async Task<List<TeamDto>> GetAllAsync()
    {
        var teams = await _context.Teams.ToListAsync();
        return teams.Select(x => x.ToDto()).ToList();
    }

    public async Task RequestJoin(int userId, int teamId, int characterId)
    {
        //TODO: validate user not have a team already
        var team = new User { Id = userId, TeamId = teamId, CharacterId = characterId, PendingAproval = true };
       this._context.Users.Attach(team);        
        //Mark only selected attributes as modified
        this._context.Entry(team).Property(x => x.TeamId).IsModified = true;
        this._context.Entry(team).Property(x => x.CharacterId).IsModified = true;
        this._context.Entry(team).Property(x => x.PendingAproval).IsModified = true;

        await this._context.SaveChangesAsync();
    }

    public async Task<List<User>> GetPendingJoinAprovals(int teamId)
    {
        return await this._context.Users.Where(x => x.TeamId == teamId && x.PendingAproval).ToListAsync();
    }

    public async Task AproveJoinRequest(int teamId, int userId, bool aprove)
    {
        var user = new User
        {
            Id = userId,
            TeamId = aprove ? teamId : null,          
        };

        if (!aprove)
        {
            user.CharacterId = null;
            user.TeamId = null;            
        }

        user.PendingAproval = false;
        this._context.Users.Attach(user);

        this._context.Entry(user).Property(x => x.TeamId).IsModified = true;
        this._context.Entry(user).Property(x => x.PendingAproval).IsModified = true;
        if (!aprove)
        {
            this._context.Entry(user).Property(x => x.CharacterId).IsModified = true;
        }

        await this._context.SaveChangesAsync();
    }

    public async Task<TeamDto> Create(string teamName, byte[] image, int? leaderId)
    {
        var team = new Team { Name = teamName, Image = image };
        this._context.Teams.Add(team);
        await this._context.SaveChangesAsync();
        if (leaderId.HasValue)
        {
            this._context.Users.Update(new User { Id = leaderId.Value, TeamId = team.Id, IsLeader = true });
            await this._context.SaveChangesAsync();
        }

        return team.ToDto();
    }

    public async Task<byte[]> GetImage(int teamId)
    {
        //TODO: Avoid load all for the image
        var team = await this._context.Teams.FindAsync(teamId);
        return team.Image;
    }

    public async Task MakeLeader(int teamId, int userId)
    {
        await _context.Database.ExecuteSqlRawAsync("UPDATE Users SET IsLeader = 0 WHERE TeamId = {0}", teamId);
        await _context.Database.ExecuteSqlRawAsync("UPDATE Users SET IsLeader = 1 WHERE TeamId = {0} AND Id = {1}", teamId, userId);
    }

    public async Task Delete(int teamId)
    {
        this._context.Remove(new Team() { Id = teamId });
        await this._context.SaveChangesAsync();
    }
}
