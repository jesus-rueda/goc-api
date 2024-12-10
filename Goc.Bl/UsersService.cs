// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business;

using System;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

public class CampaingProfile : ICampaingProfile
{
    #region Properties

    public int? CharacterId { get; set; }

    public int Id { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsLeader { get; set; }

    public int? TeamId { get; set; }

    public string Upn { get; set; }

    #endregion
}

internal class UsersService : IUserService
{
    #region Fields

    private readonly GocContext myContext;

    private readonly ICampaignService myCampaignService;

    #endregion

    #region Constructors

    public UsersService(GocContext context, ICampaignService campaignService)
    {
        this.myContext = context;
        this.myCampaignService = campaignService;
    }

    #endregion

    #region Methods

    public async Task AutoRegisterUser(string upn)
    {
        var iuser = await this.GetByUpn(upn); // hate this
        if (iuser == null)
        {
            var user = new User() { Upn = upn };
            await this.myContext.Users.AddAsync(user);
            await this.myContext.SaveChangesAsync();
        }
    }

    public async Task<ICampaingProfile> GetByUpn(string upn)
    {
        var activeCampaign = await myCampaignService.GetActive();

        var user = await this.myContext.Users
            .Where(x => x.Upn == upn)
            .FirstOrDefaultAsync();

        var membership = await this.myContext.Memberships
            .Where(x => x.CampaignId == activeCampaign.Id && x.UserId == user.Id)
            .FirstOrDefaultAsync();

        return new CampaingProfile()
               {
                   Id = user.Id,
                   TeamId = membership?.TeamId,
                   Upn = upn,
                   CharacterId = membership?.CharacterId,
                   IsAdmin = user.IsAdmin,
                   IsLeader = membership?.IsLeader ?? false
               };
    }

    #endregion
}