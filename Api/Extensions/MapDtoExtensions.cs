﻿using Goc.Api.Dtos;
using Goc.Models;

namespace Goc.Api.Extensions;

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
        return mission.Select(m => ToDto(m)).ToList();
    }

    public static TeamsDto ToDto(this Teams teams)
    {
        return new TeamsDto
        {
            Id = teams.Id,
            Name = teams.Name,
            Coinks = teams.Coinks,
        };
    }

    public static List<TeamsDto> ToDto(this List<Teams> teams)
    {
        return teams.Select(t => ToDto(t)).ToList();
    }
}
