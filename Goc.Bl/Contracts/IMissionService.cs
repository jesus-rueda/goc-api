using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts
{
    using System.IO;
    using Goc.Models;

    public interface IMissionService
    {
        Task<MissionsDto> GetAsync(int campaignId, int id);

        Task<List<MissionsDto>> GetCampaignMissionsAsync(int campaignId);
        
    }
}
