using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Models;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace Goc.Business;

public class MissionService : IMissionService
{
    private readonly GocContext myContext;

    private readonly IEvidenceService myEvidenceService;

    public MissionService(GocContext context, IEvidenceService evidenceService)
    {
        this.myContext = context;
        this.myEvidenceService = evidenceService;
    }

    public async Task<MissionsDto> GetAsync(int campaignId, int id)
    {
        var mission = await this.myContext.Missions
            .Include(x => x.MissionCampaigns)
            .ThenInclude(missionCampaign => missionCampaign.Campaign)
            .FirstOrDefaultAsync(x=>x.Id == id);

        var missionDto = mission.MissionCampaigns.FirstOrDefault(x=>x.Campaign.Id == campaignId).ToDto();
        missionDto.Status = this.GetMissionStatus(missionDto, DateTime.Now);

        return missionDto;
    }

    public async Task<List<MissionsDto>> GetCampaignMissionsAsync(int campaignId)
    {
        var missions = await this.myContext.Missions
            .Include(x => x.MissionCampaigns)
            .ThenInclude(x => x.Campaign)
            .ThenInclude(x=>x.Mission)
            .Where(x => x.Id == campaignId)
            .SelectMany(x=>x.MissionCampaigns)
            .ToListAsync();

        var missionsDto = missions.ToDto();
        missionsDto.ForEach(m =>
        {
            m.Status = this.GetMissionStatus(m, DateTime.Now);
        });

        return missionsDto;
    }

    
    
    public string GetMissionStatus(MissionsDto mission, DateTime date)
    {
        if (mission.StartDate > date) return "Planned";
        return mission.EndDate < date ? "Closed" : "Active";
    }
}
