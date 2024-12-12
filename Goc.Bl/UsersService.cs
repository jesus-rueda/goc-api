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

public class CampaignProfile : ICampaignProfile
{
    #region Properties

    public int? CharacterId { get; set; }

    public int? CampaignId { get; set; }

    public int? MembershipId { get; set; }

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

    public async Task<ICampaignProfile> GetProfileByMemberId(int memberId)
    {
        var membership = await this.myContext.Memberships
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.MembershipId == memberId);

        ; return new CampaignProfile()
                 {
                     CampaignId = membership.CampaignId,
                     MembershipId = membership?.MembershipId,
                     Id = membership.User.Id,
                     TeamId = membership?.TeamId,
                     Upn = membership.User.Upn,
                     CharacterId = membership?.CharacterId,
                     IsAdmin = membership.User.IsAdmin,
                     IsLeader = membership?.IsLeader ?? false
                 };
    }

    public async Task AutoRegisterUser(string upn)
    {
        var iuser = await this.GetProfileByUpn(upn); // hate this
        if (iuser == null)
        {
            var user = new User() { Upn = upn };
            await this.myContext.Users.AddAsync(user);
            await this.myContext.SaveChangesAsync();
        }
    }


    public async Task<ICampaignProfile> GetCampaignProfile(int? campaignId, User user)
    {
        var membership = await this.myContext.Memberships
            .Where(x => x.CampaignId == campaignId && x.UserId == user.Id)
            .FirstOrDefaultAsync();

        ; return new CampaignProfile()
          {
              CampaignId = campaignId,
            MembershipId = membership?.MembershipId,
              Id = user.Id,
              TeamId = membership?.TeamId,
              Upn = user.Upn,
              CharacterId = membership?.CharacterId,
              IsAdmin = user.IsAdmin,
              IsLeader = membership?.IsLeader ?? false
          };
    }


    public async Task<ICampaignProfile> GetProfileByUpn(string upn)
    {
        var user = await this.myContext.Users
            .Where(x => x.Upn == upn)
            .FirstOrDefaultAsync();

        if(user== null)
        {
            return null;
        }

        var activeCampaign = await this.myCampaignService.GetActive();
        var campaignId = activeCampaign?.Id;
        var profile =await this.GetCampaignProfile(campaignId, user);
        return profile;
    }

    #endregion
}