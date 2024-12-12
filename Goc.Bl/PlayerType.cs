// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business;

internal enum PlayerType
{
    Challenger,
    Defender
}

public enum GameResult
{
    ChallengerWin,
    DefenderWin,
    Draw
}


public enum PlayerGameResult
{
    Win = 0,
    Lose = 1,
    Draw = 2
}