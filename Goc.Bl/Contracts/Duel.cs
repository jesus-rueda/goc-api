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

public class Duel
{
    public int Id { get; set; }

    public bool Effective { get; set; }

    public string Message { get; set; }

    public DateTime Deadline { get; set; }

    public int Coinks { get; set; }

    public TeamCharacterProfileDto Oponent { get; set; }
}