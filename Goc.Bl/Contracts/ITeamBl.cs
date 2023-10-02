using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts
{
    using Goc.Business.Dtos;
    using System;

    public interface ITeamBl
    {
        Task<List<TeamDto>> GetAllAsync();

        Task<TeamDto> GetAsync(int id);

        Task<TeamMission> GetMissionProgressAsync(int missionId, int teamId);

        Task<TeamMemberStats> GetTeamMemberStatsAsync(int memberId);
        Task RequestJoin(int userId, int teamId, int characterId);

        Task<List<User>> GetPendingJoinAprovals(int teamId);

        Task AproveJoinRequest(int teamId, int userId, bool aprove);
        Task<TeamDto> Create(string teamName, byte[] image, int? leaderId);
        Task<byte[]> GetImage(int teamId);
        Task MakeLeader(int teamId, int userId);
        Task Delete(int teamId);
    }
}