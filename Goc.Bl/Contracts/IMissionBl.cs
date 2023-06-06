using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts
{
    public interface IMissionBl
    {
        Task<MissionsDto> GetAsync(int id);

        Task<List<MissionsDto>> GetCampaignMissionsAsync(int campaignId);
    }
}
