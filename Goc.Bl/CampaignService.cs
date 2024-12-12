using System;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

using System.Collections.Generic;

public class CampaignService : ICampaignService
{

    private readonly Dictionary<ActionType, CampaignActionParameters> FixedParams = new()
                                                                                    {
                                                                                        {
                                                                                            ActionType.CompleteMission,
                                                                                            new CampaignActionParameters()
                                                                                            {
                                                                                                ActionType = ActionType.CompleteMission,
                                                                                                Duration = TimeSpan.FromDays(2)
                                                                                            }
                                                                                        },
                                                                                        {
                                                                                            ActionType.Attack,
                                                                                            new CampaignActionParameters()
                                                                                            {
                                                                                                ActionType = ActionType.Attack,
                                                                                                MaxAllowed = 5,
                                                                                                Coinks = 500,
                                                                                                Duration = TimeSpan.FromDays(2)
                                                                                            }
                                                                                        },
                                                                                        {
                                                                                            ActionType.SetupDefence,
                                                                                            new CampaignActionParameters()
                                                                                            {
                                                                                                ActionType = ActionType.SetupDefence,
                                                                                                MaxAllowed = 1,
                                                                                                Coinks = 0,
                                                                                                Duration = TimeSpan.FromDays(2)
                                                                                            }
                                                                                        },
                                                                                        {
                                                                                            ActionType.DuelChallenge,
                                                                                            new CampaignActionParameters()
                                                                                            {
                                                                                                ActionType = ActionType.DuelChallenge,
                                                                                                Coinks = 2000,
                                                                                                MaxAllowed = 5,
                                                                                                Duration = TimeSpan.FromDays(2)
                                                                                            }
                                                                                        },

                                                                                    };

    private readonly GocContext _context;

    public CampaignService(GocContext context)
    {
        _context = context;
    }

    public async Task<Campaign> GetActive()
    {
        return await _context.Campaigns.FirstOrDefaultAsync(c =>
            c.StartDate <= DateTime.UtcNow && c.EndDate >= DateTime.UtcNow);
    }


    public Task<CampaignActionParameters> GetParametersFor(int campaignId, ActionType type)
    {
        // this should be configurable by campaign in the db.
       return Task.FromResult(this.FixedParams[type]);
    }
}