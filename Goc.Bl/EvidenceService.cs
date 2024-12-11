// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business;

using System;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

public class EvidenceService : IEvidenceService
{
    #region Fields

    private readonly GocContext myContext;

    #endregion

    #region Constructors

    public EvidenceService(GocContext context)
    {
        this.myContext = context;
    }

    #endregion

    #region Methods

    public async Task<long> RegisterAsync(
        int campaignId,
        ActionType action,
        int membershipId,
        int coinks,
        TimeSpan duration,
        int? affectedTeamId = null,
        int? missionId = null,
        byte[]? evidence = null)
    {
        var date = DateTime.Now;
        await using var transaction = await this.myContext.Database.BeginTransactionAsync();

        var actionLog = new ActionLog()
                        {
                            MissionId = missionId,
                            //TeamId = teamId,
                            TeamCharacterId = membershipId,
                            ActionTypeId = (int)action,
                            AffectedTeamId = affectedTeamId,
                            DateTimeTo = date,
                            DateTimeFrom = date.Add(duration),
                            Coinks = coinks
                        };

        this.myContext.ActionsLog.Add(actionLog);

        if (evidence != null)
        {
            await this.myContext.SaveChangesAsync();
            var ev = new Evidence()
            {
                ActionLogId = actionLog.Id,
                TeamCharacterId = actionLog.TeamCharacterId,
                Image = Convert.ToBase64String(evidence),
                IsValid = true //TODO: remove  auto validate the evidences
            };
            this.myContext.Evidences.Add(ev);
        }

        // Check if update team coinks balance.
        await this.UpdateCoinksBalance(actionLog);

        await this.myContext.SaveChangesAsync();

        await transaction.CommitAsync();
        return actionLog.Id;
    }

    internal async Task UpdateCoinksBalance(ActionLog log)
    {
        if (log.Coinks != 0)
        {
            if (log.AffectedTeamId != null)
            {
                var affectedTeam = await this.myContext.Teams.FindAsync(log.AffectedTeamId);
                affectedTeam.Coinks -= log.Coinks;
            }

            var membership = await this.myContext.Memberships.Include(m => m.Team)
                .FirstOrDefaultAsync(m => m.MembershipId == log.TeamCharacterId);

            membership.Team.Coinks += log.Coinks;
        }
    }

    #endregion
}