using System;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class CampaignBl : ICampaignBl
{
    private readonly GocContext _context;

    public CampaignBl(GocContext context)
    {
        _context = context;
    }

    public async Task<Campaigns> GetActive()
    {
        return await _context.Campaigns.FirstOrDefaultAsync(c =>
            c.StartDate <= DateTime.UtcNow && c.EndDate >= DateTime.UtcNow);
    }
}