// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Goc.Models;


public static class ClaimsPrincipalExtensions
{
    public static ICampaingProfile GetGocUser(this ClaimsPrincipal principal)
    {
        return principal.Identities.OfType<GocIdentity>().FirstOrDefault()?.User;
    }
}

public class GocIdentity : ClaimsIdentity
{
    public ICampaingProfile User { get; }
    public GocIdentity(ICampaingProfile user, List<Claim> claims):base(claims)
    {
        this.User = user;
    }
}