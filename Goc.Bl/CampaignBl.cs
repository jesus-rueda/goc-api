using System.Collections.Generic;
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

    public async Task<List<Campaigns>> GetAll()
    {
        var campaigns = await _context.Campaigns.ToListAsync();
        return campaigns;
    }
}