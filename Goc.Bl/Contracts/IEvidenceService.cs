using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts;

using System;
using System.IO;

public interface IEvidenceService
{
    public Task<long> RegisterAsync(int campaignId, ActionType action, int membershipId, int coinks, TimeSpan duration, int? affectedTeamId = null, int? missionId = null, byte[]? evidence = null);
}
