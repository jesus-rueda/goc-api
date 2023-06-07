using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts
{
    using Goc.Business.Dtos;

    public interface ITeamBl
    {
        Task<List<Teams>> GetAllAsync();

        Task<TeamsDto> GetAsync(int id);

        Task<TeamMission> GetMissionProgressAsync(int missionId, int teamId);

        Task<TeamMemberStats> GetTeamMemberStatsAsync(int memberId);
    }
}
