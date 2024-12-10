using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts;

public interface IEvidenceService
{
    Task<List<EvidencesDto>> GetAllAsync();

    Task<EvidencesDto> GetAsync(int id);

    Task<EvidencesDto> CreateAsync(int campaignId, int missionId, int teamId, int actionId, int userId, int? affectedTeamId, string image);
}
