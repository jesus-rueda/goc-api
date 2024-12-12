using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Contracts
{
    using Goc.Models;

    public interface IUserService
    {
        Task<ICampaignProfile> GetProfileByUpn(string upn);
        Task<ICampaignProfile> GetProfileByMemberId(int memberId);


        Task AutoRegisterUser(string upn);

        Task<ICampaignProfile> GetCampaignProfile(int? campaignId, User user);
    }
}
