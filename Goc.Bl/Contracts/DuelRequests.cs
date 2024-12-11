// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business.Contracts;

public class DuelRequests
{
    public int TargetTeamId { get; set; }

    public int BetCoinks { get; set; }

    public int GameId { get; set; }
}