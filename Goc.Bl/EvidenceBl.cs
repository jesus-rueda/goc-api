using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Models;

namespace Goc.Business;

public class EvidenceBl : IEvidenceBl
{
    //const int COINKS_MISSION = 500;
    const int COINKS_ATTACK = 500;
    const int COINKS_DEFENSE = 0;
    const int COINKS_BONNUS = 500;

    private readonly GocContext _context;

    public EvidenceBl(GocContext context)
    {
        this._context = context;
    }

    public async Task<EvidencesDto> CreateAsync(int missionId, int teamId, int actionId, int teamCharacterId, int? affectedTeamId, string image)
    {
        //Validations
        var mission = await _context.Missions.FindAsync(missionId);
        if (mission == null)
        {
            throw new Exception("Mission not found");
        }

        var team = await _context.Teams.FindAsync(teamId);
        if (team == null)
        {
            throw new Exception("team not found");
        }

        if (actionId == 1) // Attack
        {
            var affectedTeam = await _context.Teams.FindAsync(affectedTeamId);
            if (affectedTeam == null)
            {
                throw new Exception("Affected team not found");
            }
        }

        var teamCharacter = await _context.TeamsCharacters.FindAsync(teamCharacterId);
        if (teamCharacter == null)
        {
            throw new Exception("Character not found");
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
                MissionId = missionId,
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
}
