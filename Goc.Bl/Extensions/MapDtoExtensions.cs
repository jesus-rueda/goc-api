using System.Collections.Generic;
using System.Linq;
using Goc.Business.Dtos;
using Goc.Models;

namespace Goc.Business.Extensions;

public static class MapDtoExtensions
{
    public static MissionsDto ToDto(this MissionCampaign mission)
    {
        return new MissionsDto
        {
            Id = mission.Mission.Id,
            Name = mission.Mission.Name,
            Story = mission.Mission.Story,
            Instructions = mission.Mission.Instructions,
            Coinks = mission.Mission.Coinks,
            StartDate = mission.StartDate,
            EndDate = mission.EndDate
        };
    }

    public static List<MissionsDto> ToDto(this List<MissionCampaign> mission)
    {
        return mission.Select(m => m.ToDto()).ToList();
    }

    public static TeamDto ToDto(this Team teams)
    {
        
        return new TeamDto
        {
            Id = teams.Id,
            Name = teams.Name,
            Coinks = teams.Coinks,
            Members  = teams.TeamsCharacters.Select(x=> x.ToDto()).AsEnumerable().ToArray()
        };
    }

    public static List<TeamDto> ToDto(this List<Team> teams)
    {
        return teams.Select(t => t.ToDto()).ToList();
    }

    public static TeamCharacterProfileDto ToDto(this Membership teamCharacter)
    {
        if (teamCharacter == null)
        {
            return null;
        }

        return new TeamCharacterProfileDto
        {
            Id = teamCharacter.User.Id,
            Upn = teamCharacter.User.Upn,
            CharacterId = teamCharacter.CharacterId ?? 0,
            CharacterName = teamCharacter.Character?.Name,
            IsLeader = teamCharacter.IsLeader ? 1 : 0,
            TeamId = teamCharacter.TeamId ?? 0,
            TeamName = teamCharacter.Team?.Name,
            IsPendingAproval = teamCharacter.PendingAproval
        };
    }

    public static EvidencesDto ToDto(this Evidence evidence)
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

    public static List<EvidencesDto> ToDto(this List<Evidence> evidences)
    {
        return evidences.Select(t => t.ToDto()).ToList();
    }

    public static MessagesDto ToDto(this Message message)
    {
        return new MessagesDto
        {
            Id = message.Id,
            SenderTeam = message.SenderTeam,
            RecipientTeam = message.RecipientTeam,
            Message = message.Text,
            DateTime = message.DateTime,
        };
    }

    public static List<MessagesDto> ToDto(this List<Message> messages)
    {
        return messages.Select(m => m.ToDto()).ToList();
    }

    public static Message ToEntity(this MessagesDto message)
    {
        return new Message
        {
            Id = message.Id,
            SenderTeam = message.SenderTeam,
            RecipientTeam = message.RecipientTeam,
            Text = message.Message,
            DateTime = message.DateTime,
        };
    }
}
