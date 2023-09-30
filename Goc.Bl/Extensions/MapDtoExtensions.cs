using System.Collections.Generic;
using System.Linq;
using Goc.Business.Dtos;
using Goc.Models;

namespace Goc.Business.Extensions;

public static class MapDtoExtensions
{
    public static MissionsDto ToDto(this Missions mission)
    {
        return new MissionsDto
        {
            Id = mission.Id,
            Name = mission.Name,
            Story = mission.Story,
            Instructions = mission.Instructions,
            Coinks = mission.Coinks,
            StartDate = mission.StartDate,
            EndDate = mission.EndDate
        };
    }

    public static List<MissionsDto> ToDto(this List<Missions> mission)
    {
        return mission.Select(m => m.ToDto()).ToList();
    }

    public static TeamsDto ToDto(this Teams teams)
    {
        
        return new TeamsDto
        {
            Id = teams.Id,
            Name = teams.Name,
            Coinks = teams.Coinks,
            Members  = teams.TeamsCharacters.Select(x=> x.ToDto()).AsEnumerable().ToArray()
        };
    }

    public static List<TeamsDto> ToDto(this List<Teams> teams)
    {
        return teams.Select(t => t.ToDto()).ToList();
    }

    public static TeamCharacterProfileDto ToDto(this User teamCharacter)
    {
        if (teamCharacter == null)
        {
            return null;
        }

        return new TeamCharacterProfileDto
        {
            Id = teamCharacter.Id,
            Email = teamCharacter.Upn,
            CharacterId = teamCharacter.CharacterId??0,
            CharacterName = teamCharacter.Character?.Name,
            IsLeader = teamCharacter.IsLeader ? 1 : 0,
            TeamId = teamCharacter.TeamId ?? 0,
            TeamName = teamCharacter.Team?.Name
        };
    }

    public static EvidencesDto ToDto(this Evidences evidence)
    {
        return new EvidencesDto
        {
            Id = evidence.Id,
            ActionLogId = evidence.ActionLogId,
            TeamCharacterId = evidence.TeamCharacterId,
            Image = evidence.Image,
            IsValid = evidence.IsValid,
        };
    }

    public static List<EvidencesDto> ToDto(this List<Evidences> evidences)
    {
        return evidences.Select(t => t.ToDto()).ToList();
    }

    public static MessagesDto ToDto(this Messages message)
    {
        return new MessagesDto
        {
            Id = message.Id,
            SenderTeam = message.SenderTeam,
            RecipientTeam = message.RecipientTeam,
            Message = message.Message,
            DateTime = message.DateTime,
        };
    }

    public static List<MessagesDto> ToDto(this List<Messages> messages)
    {
        return messages.Select(m => m.ToDto()).ToList();
    }

    public static Messages ToEntity(this MessagesDto message)
    {
        return new Messages
        {
            Id = message.Id,
            SenderTeam = message.SenderTeam,
            RecipientTeam = message.RecipientTeam,
            Message = message.Message,
            DateTime = message.DateTime,
        };
    }
}
