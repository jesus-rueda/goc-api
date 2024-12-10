using System;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class CampaignService : ICampaignService
{
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
}