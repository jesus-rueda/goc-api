// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business.Contracts;

using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;
using Goc.Models;

public interface ITeamService
{
    #region Methods

    Task ApproveJoinRequest(int campaignId, int teamId, int userId, bool approve);

    Task<TeamDto> Create(string teamName, byte[] image, int? leaderId);

    Task Delete(int teamId);

    Task<List<TeamDto>> GetAllAsync(int campaignId);

    Task<TeamDto> GetAsync(int campaignId, int id);

    Task<byte[]> GetImage(int teamId);

    Task<TeamMission> GetMissionProgressAsync(int campaignId, int missionId, int teamId);

    Task<List<Membership>> GetPendingJoinApprovals(int campaignId, int teamId);

    Task<TeamMemberStats> GetTeamMemberStatsAsync(int campaignId, int userId);

    Task MakeLeader(int campaignId, int teamId, int userId);

    Task RequestJoin(int campaignId, int userId, int teamId, int characterId);

    #endregion
}