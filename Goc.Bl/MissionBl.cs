using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class MissionBl : IMissionBl
{
    private readonly GocContext _context;

    public MissionBl(GocContext context)
    {
        _context = context;
    }

    public async Task<MissionsDto> GetAsync(int id)
    {
        var mission = await _context.Missions.FindAsync(id);

        var missionDto = mission.ToDto();
        missionDto.Status = GetMissionStatus(missionDto, DateTime.Now);

        return missionDto;
    }

    public async Task<List<MissionsDto>> GetCampaignMissionsAsync(int campaignId)
    {
        var missions = await _context.Missions.Where(c => c.Campaign.FirstOrDefault().Id == campaignId).ToListAsync();
        var missionsDto = missions.ToDto();
        missionsDto.ForEach(m =>
        {
            m.Status = GetMissionStatus(m, DateTime.Now);
        });

        return missionsDto;
    }

    public string GetMissionStatus(MissionsDto mission, DateTime date)
    {
        if (mission.StartDate > date) return "Planned";
        return mission.EndDate < date ? "Closed" : "Active";
    }
}
