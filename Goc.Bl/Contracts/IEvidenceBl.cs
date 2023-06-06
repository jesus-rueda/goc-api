using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts;

public interface IEvidenceBl
{
    Task<List<EvidencesDto>> GetAllAsync();

    Task<EvidencesDto> GetAsync(int id);

    Task<EvidencesDto> CreateAsync(int missionId, int teamId, int actionId, int teamCharacterId, int? affectedTeamId, string image);
}
