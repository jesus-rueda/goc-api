using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Business.Services;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class EvidenceBl : IEvidenceBl
{
    //const int COINKS_MISSION = 500;
    const int COINKS_ATTACK = 500;
    const int COINKS_DEFENSE = 0;
    const int COINKS_BONNUS = 500;

    private readonly GocContext _context;
    private readonly IMessageBl _messageBl;


    public EvidenceBl(GocContext context, IMessageBl messageBl)
    {
        this._context = context;
        this._messageBl = messageBl;
    }

    public async Task<EvidencesDto> CreateAsync(int missionId, int teamId, int actionId, int teamCharacterId, int? affectedTeamId, string image)
    {
        //Validations
        var mission = await this.GetMission(missionId);

        var team = await _context.Teams.FindAsync(teamId);
        if (team == null)
        {
            throw new Exception("team not found");
        }
        
        var teamCharacter = await _context.TeamsCharacters.FindAsync(teamCharacterId);
        if (teamCharacter == null)
        {
            throw new Exception("Character not found");
        }

        if (actionId == 2) // Attack
        {
            var affectedTeam = await _context.Teams.FindAsync(affectedTeamId);
            if (affectedTeam == null)
            {
                throw new Exception("Affected team not found");
            }

            // El equipo atacado tiene defensa?
            var isAffectedTeamDefended =
                await _context.ActionsLog.AnyAsync(a => a.MissionId == missionId && a.TeamId == affectedTeamId && a.ActionTypeId == 3);

            if (isAffectedTeamDefended)
            {
                var messageTemplate = await _context.MessageTemplates.FirstOrDefaultAsync(mt => mt.ActionTypeId == 3);
                await _messageBl.CreateAsync(
                    new MessagesDto
                    {
                        DateTime = DateTime.UtcNow,
                        Message = messageTemplate.Body.Replace("<attackedteam>", affectedTeam.Name),
                        RecipientTeam = teamId,
                        SenderTeam = affectedTeam.Id
                    });

                throw new Exception("Attack not effective");
            }
            else
            {
                var characterSkills = await _context.Characters.FirstOrDefaultAsync(c => c.Id == teamCharacter.CharacterId);
                var messageTemplate = await _context.MessageTemplates.FirstOrDefaultAsync(mt => mt.ActionTypeId == 2);
                await _messageBl.CreateAsync(
                    new MessagesDto
                    {
                        DateTime = DateTime.UtcNow,
                        Message = messageTemplate.Body.Replace("<attacker>", team.Name).Replace("<attackdescription>", characterSkills.Attack),
                        RecipientTeam = affectedTeam.Id,
                        SenderTeam = teamId
                    });
            }
        }

        var action = await _context.ActionTypes.FindAsync(actionId);
        if (action == null)
        {
            throw new Exception("Action type not found");
        }

        var date = DateTime.Now;
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var actionLog = new ActionsLog()
            {
                MissionId = mission.Id,
                TeamId = teamId,
                TeamCharacterId = teamCharacterId,
                ActionTypeId = actionId,
                AffectedTeamId = action.Id == 1 ? affectedTeamId : null,
                DateTimeTo = date,
                DateTimeFrom = date.AddDays(2),
                Coinks = GetCoinks(action, mission)
            };

            _context.ActionsLog.Add(actionLog);

            var rowAffected = await _context.SaveChangesAsync();
            if (rowAffected == 0) throw new Exception("The evidence could not be created");

            var evidence = new Evidences()
            {
                ActionLogId = actionLog.Id,
                TeamCharacterId = actionLog.TeamCharacterId,
                Image = image,
                IsValid = true
            };

            _context.Evidences.Add(evidence);

            // Check if update team coinks balance is required.
            await this.UpdateCoinksBalance(team, mission);

            rowAffected = await _context.SaveChangesAsync();
            if (rowAffected == 0) throw new Exception("The evidence could not be created");

            transaction.Commit();
            return evidence.ToDto();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public Task<List<EvidencesDto>> GetAllAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<EvidencesDto> GetAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public int GetCoinks(ActionTypes actionType, Missions mission)
    {
        int coinks;
        switch (actionType.Id)
        {
            case 1:
                coinks = mission.Coinks;
                break;
            case 2:
                coinks = COINKS_ATTACK;
                break;
            case 3:
                coinks = COINKS_DEFENSE;
                break;
            case 4:
                coinks = COINKS_BONNUS;
                break;
            default:
                coinks = 0;
                break;
        }

        return coinks;
    }

    internal async Task UpdateCoinksBalance(Teams team, Missions mission)
    {
        var teamCharactersCount = await _context.TeamsCharacters.Where(c => c.TeamId == team.Id).CountAsync();

        var actionsCount = await _context.ActionsLog
            .Include(a => a.Evidences)
            .Where(a => a.TeamId == team.Id && a.MissionId == mission.Id && a.Evidences.FirstOrDefault().IsValid)
            .GroupBy(a => a.TeamCharacterId)
            .CountAsync();

        if (actionsCount != teamCharactersCount - 1)
        {
            return;
        }

        team.Coinks += mission.Coinks;
    }

    private async Task<Missions> GetMission(int missionId)
    {
        if (missionId <= 0)
        {
            return await _context.Missions.FirstOrDefaultAsync(m =>
                m.StartDate <= DateTime.UtcNow && m.EndDate >= DateTime.UtcNow);
        }

        var mission = await _context.Missions.FindAsync(missionId);
        if (mission == null)
        {
            throw new Exception("Mission not found");
        }

        return mission;
    }
}
