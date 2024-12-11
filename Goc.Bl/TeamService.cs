// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

public class TeamService : ITeamService
{
    #region Fields

    private readonly GocContext _context;

    private readonly ICampaignService myCampaignService;

    #endregion

    #region Constructors

    public TeamService(GocContext context, ICampaignService campaignService)
    {
        this._context = context;
        this.myCampaignService = campaignService;
    }

    #endregion

    #region Methods

    public async Task ApproveJoinRequest(int campaignId, int teamId, int userId, bool approve)
    {
        var membership = new Membership() { UserId = userId, TeamId = approve ? teamId : null, CampaignId = campaignId };

        if (!approve)
        {
            membership.CharacterId = null;
            membership.TeamId = null;
        }

        membership.PendingAproval = false;
        this._context.Memberships.Attach(membership);

        this._context.Entry(membership)
            .Property(x => x.TeamId)
            .IsModified = true;
        this._context.Entry(membership)
            .Property(x => x.PendingAproval)
            .IsModified = true;
        if (!approve)
        {
            this._context.Entry(membership)
                .Property(x => x.CharacterId)
                .IsModified = true;
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
            this._context.Memberships.Update(new Membership() { UserId = leaderId.Value, TeamId = team.Id, IsLeader = true });
            await this._context.SaveChangesAsync();
        }

        return team.ToDto();
    }

    public async Task Delete(int teamId)
    {
        await this._context.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Teams WHERE Id = {0}", teamId);
    }

    public async Task<List<TeamDto>> GetAllAsync(int campaignId)
    {
        var teams = await this._context.Teams
            .Include(x=>x.TeamsCharacters.Where(x=>x.CampaignId == campaignId))
            .ThenInclude(x=>x.User)
            .ToListAsync();


        return teams.Select(x => x.ToDto())
            .ToList();
    }

    public async Task<TeamDto> GetAsync(int campaignId, int id)
    {
        var team = await this._context.Teams
            .Include(x => x.TeamsCharacters.Where(x=>x.CampaignId == campaignId))
            .ThenInclude(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id == id);
            

        var dto = team.ToDto();

        var teamMembersCount = await this._context.Memberships.CountAsync(x => x.TeamId == id && !x.PendingAproval);

        dto.AttacksDone = await this._context.ActionsLog
            .Include(x=>x.TeamCharacter)
            .CountAsync(x => x.TeamCharacter.TeamId == id && x.ActionTypeId == (int)ActionType.Attack);

        var attackPars = await this.myCampaignService.GetParametersFor(campaignId, ActionType.Attack);
        var deffensePars = await this.myCampaignService.GetParametersFor(campaignId, ActionType.SetupDefence);

        dto.AttacksTotal = teamMembersCount * attackPars.MaxAllowed ?? 0;
        dto.DefensesTotal = teamMembersCount * deffensePars.MaxAllowed ?? 0;


        dto.DefensesUsed = await this._context.ActionsLog
            .Include(x=>x.TeamCharacter)
            .CountAsync(x => x.TeamCharacter.TeamId == id && x.ActionTypeId == (int)ActionType.SetupDefence);

        return dto;
    }

    public async Task<byte[]> GetImage(int teamId)
    {
        //TODO: Avoid load all for the image
        var team = await this._context.Teams.FindAsync(teamId);
        return team.Image;
    }

    public async Task<TeamMission> GetMissionProgressAsync(int campaignId, int missionId, int teamId)
    {
        var total = await this._context.Memberships.CountAsync(c => c.TeamId == teamId && c.CampaignId == campaignId);

        var actionsCount = await this._context.ActionsLog
            .Include(x=> x.TeamCharacter)
            .Include(a => a.Evidences)
            .Where(
                   a => 
                           a.TeamCharacter.TeamId == teamId
                        && a.MissionId == missionId 
                        && a.TeamCharacter.CampaignId == campaignId
                        && a.Evidences.Any(e=>e.IsValid))
            .GroupBy(a => a.TeamCharacterId)
            .CountAsync();

        var progress = 100 * actionsCount / total;

        return new TeamMission
               {
                   MissionCompleteness = progress
               };
    }

    public async Task<List<Membership>> GetPendingJoinApprovals(int campaignId, int teamId)
    {
        return await this._context.Memberships.Where(x => x.TeamId == teamId && x.PendingAproval && x.CampaignId == campaignId)
            .ToListAsync();
    }

    public async Task<TeamMemberStats> GetTeamMemberStatsAsync(int campaignId, int userId)
    {
        

        var attacks = await this._context.ActionsLog
            .Include(x=>x.TeamCharacter)
            .CountAsync(a => a.TeamCharacter.UserId == userId 
                             && a.TeamCharacter.CampaignId == campaignId
                             && a.ActionTypeId == (int)ActionType.Attack);

        var defends = await this._context.ActionsLog.Include(x => x.TeamCharacter)
            .CountAsync(a => a.TeamCharacter.UserId == userId && a.TeamCharacter.CampaignId == campaignId && a.ActionTypeId == (int)ActionType.SetupDefence);

        var attackPars = await this.myCampaignService.GetParametersFor(campaignId, ActionType.Attack);
        var deffensePars = await this.myCampaignService.GetParametersFor(campaignId, ActionType.SetupDefence);

        return new TeamMemberStats { AttacksDone = attacks, AttacksTotal = attackPars.MaxAllowed ?? 0, DefensesTotal = deffensePars.MaxAllowed ?? 0, DefensesUsed = defends };
    }

    public async Task MakeLeader(int campaignId, int teamId, int userId)
    {
        await this._context.Database.ExecuteSqlRawAsync("UPDATE TeamsCharacters SET IsLeader = 0 WHERE TeamId = {0} AND campaignId = {1}", teamId, campaignId);
        await this._context.Database.ExecuteSqlRawAsync(
                                                        "UPDATE TeamsCharacters SET IsLeader = 1, PendingAproval = 0 WHERE TeamId = {0} AND UserId = {1} AND campaignId = {2}",
                                                        teamId,
                                                        userId,
                                                        campaignId);
    }

    public async Task RequestJoin(int campaignId, int userId, int teamId, int characterId)
    {
        //TODO: validate user not have a team already
        var team = new Membership()
                   {
                       UserId = userId,
                       TeamId = teamId,
                       CampaignId = campaignId,
                       CharacterId = characterId,
                       PendingAproval = true,
                       IsLeader = false
                   };
        this._context.Memberships.Attach(team);
        //Mark only selected attributes as modified
        this._context.Entry(team)
            .Property(x => x.TeamId)
            .IsModified = true;
        this._context.Entry(team)
            .Property(x => x.CharacterId)
            .IsModified = true;
        this._context.Entry(team)
            .Property(x => x.PendingAproval)
            .IsModified = true;
        this._context.Entry(team)
            .Property(x => x.IsLeader)
            .IsModified = true;

        await this._context.SaveChangesAsync();
    }

    #endregion
}