// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

public class EvidenceService : IEvidenceService
{
    #region Fields

    //const int COINKS_MISSION = 500;
    private const int COINKS_ATTACK = 500;

    private const int COINKS_BONNUS = 500;

    private const int COINKS_DEFENSE = 0;

    private readonly GocContext myContext;

    private readonly IMessageService myMessageService;

    #endregion

    #region Constructors

    public EvidenceService(GocContext context, IMessageService messageService)
    {
        this.myContext = context;
        this.myMessageService = messageService;
    }

    #endregion

    #region Methods

    public async Task<EvidencesDto> CreateAsync(int campaignId, int missionId, int teamId, int actionId, int userId, int? affectedTeamId, string image)
    {
        //Validations
        var mission = await this.GetMission(missionId);

        var team = await this.myContext.Teams.FindAsync(teamId);
        if (team == null)
        {
            throw new Exception("team not found");
        }

        var membership = await this.myContext.Memberships.FirstOrDefaultAsync(x => x.UserId == userId && x.CampaignId == campaignId);

        if (membership == null)
        {
            throw new Exception("Character not found");
        }

        if (actionId == 2) // Attack
        {
            var affectedTeam = await this.myContext.Teams.FindAsync(affectedTeamId);
            if (affectedTeam == null)
            {
                throw new Exception("Affected team not found");
            }

            // El equipo atacado tiene defensa?
            var isAffectedTeamDefended =
                await this.myContext.ActionsLog.AnyAsync(a => a.MissionId == mission.Id && a.TeamId == affectedTeamId && a.ActionTypeId == 3);

            if (isAffectedTeamDefended)
            {
                var messageTemplate = await this.myContext.MessageTemplates.FirstOrDefaultAsync(mt => mt.ActionTypeId == 3);
                await this.myMessageService.CreateAsync(
                                                        new MessagesDto
                                                        {
                                                            DateTime = DateTime.UtcNow,
                                                            Message = messageTemplate.Body.Replace("<attackedteam>", affectedTeam.Name),
                                                            RecipientTeam = teamId,
                                                            SenderTeam = affectedTeam.Id
                                                        });

                throw new Exception("Attack not effective, this team has an active defense!");
            }
            else
            {
                var characterSkills = await this.myContext.Characters.FirstOrDefaultAsync(c => c.Id == membership.CharacterId);
                var messageTemplate = await this.myContext.MessageTemplates.FirstOrDefaultAsync(mt => mt.ActionTypeId == 2);
                await this.myMessageService.CreateAsync(
                                                        new MessagesDto
                                                        {
                                                            DateTime = DateTime.UtcNow,
                                                            Message = messageTemplate.Body.Replace("<attacker>", team.Name)
                                                                .Replace("<attackdescription>", characterSkills.Attack),
                                                            RecipientTeam = affectedTeam.Id,
                                                            SenderTeam = teamId
                                                        });
            }
        }

        var action = await this.myContext.ActionTypes.FindAsync(actionId);
        if (action == null)
        {
            throw new Exception("Action type not found");
        }

        var date = DateTime.Now;
        await using var transaction = await this.myContext.Database.BeginTransactionAsync();

        var actionLog = new ActionLog()
                        {
                            MissionId = mission.Id,
                            TeamId = teamId,
                            TeamCharacterId = membership.MembershipId,
                            ActionTypeId = actionId,
                            AffectedTeamId = action.Id == 1 ? affectedTeamId : null,
                            DateTimeTo = date,
                            DateTimeFrom = date.AddDays(2),
                            Coinks = this.GetCoinks(action, mission)
                        };

        this.myContext.ActionsLog.Add(actionLog);

        var rowAffected = await this.myContext.SaveChangesAsync();
        if (rowAffected == 0)
        {
            throw new Exception("The evidence could not be created");
        }

        var evidence = new Evidence() { ActionLogId = actionLog.Id, TeamCharacterId = actionLog.TeamCharacterId, Image = image, IsValid = true };

        this.myContext.Evidences.Add(evidence);

        // Check if update team coinks balance is required.
        await this.UpdateCoinksBalance(team, mission, affectedTeamId, actionId);

        rowAffected = await this.myContext.SaveChangesAsync();
        if (rowAffected == 0)
        {
            throw new Exception("The evidence could not be created");
        }

        await transaction.CommitAsync();
        return evidence.ToDto();
    }

    public Task<List<EvidencesDto>> GetAllAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<EvidencesDto> GetAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public int GetCoinks(ActionType actionType, Mission mission)
    {
        int coinks;
        switch (actionType.Id)
        {
            case 1:
                coinks = mission.Coinks;
                break;
            case 2:
                coinks = EvidenceService.COINKS_ATTACK;
                break;
            case 3:
                coinks = EvidenceService.COINKS_DEFENSE;
                break;
            case 4:
                coinks = EvidenceService.COINKS_BONNUS;
                break;
            default:
                coinks = 0;
                break;
        }

        return coinks;
    }

    private async Task<Mission> GetMission(int missionId)
    {
        //if (missionId <= 0)
        //{
        //    return await this.myContext.Missions.FirstOrDefaultAsync(m =>
        //                                                                 m.StartDate <= DateTime.UtcNow && m.EndDate >= DateTime.UtcNow);
        //}

        var mission = await this.myContext.Missions.Include(m => m.Campaigns)
            .FirstOrDefaultAsync(x => x.Id == missionId);

        if (mission == null)
        {
            throw new Exception("Mission not found");
        }

        return mission;
    }

    internal async Task UpdateCoinksBalance(Team team, Mission mission, int? affectedTeamId, int actionId)
    {
        var teamCharactersCount = await this.myContext.Memberships.Where(c => c.TeamId == team.Id)
            .CountAsync();

        var actionsCount = await this.myContext.ActionsLog.Include(a => a.Evidences)
            .Where(
                   a => a.TeamId == team.Id && a.MissionId == mission.Id && a.Evidences.FirstOrDefault()
                       .IsValid)
            .GroupBy(a => a.TeamCharacterId)
            .CountAsync();

        if (actionsCount != teamCharactersCount - 1)
        {
            return;
        }

        team.Coinks += mission.Coinks;

        if (actionId == 2) // Attack
        {
            var affectedTeam = await this.myContext.Teams.FindAsync(affectedTeamId);
            if (affectedTeam != null)
            {
                affectedTeam.Coinks -= mission.Coinks;
            }
        }
    }

    #endregion
}