// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business.Contracts;

using System;
using Goc.Business.Dtos;
using Goc.Models;

public class DuelAction
{
    public int Id { get; set; }

    public bool Effective { get; set; }

    public string Message { get; set; }

    public DateTime Deadline { get; set; }

    public int Coinks { get; set; }

    public TeamCharacterProfileDto Oponent { get; set; }

    public string GameState { get; set; }

    public string GameId { get; set; }

    public bool IsMyTurn { get; set; }

    public int CurrentTurnMembershipId { get; set; }

    public int? WinnerMemberId { get; set; }
}