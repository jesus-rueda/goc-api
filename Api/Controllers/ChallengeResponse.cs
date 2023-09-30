// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Api.Controllers;

using System;

public class ChallengeResponse
{
    #region Properties

    public Guid ChallengeId { get; set; } = Guid.NewGuid();

    public string Code { get; set; }

    public int Interval { get; set; }

    public string VerificationUri { get; set; }

    public string VerificationUriComplete { get; set; }

    #endregion
}