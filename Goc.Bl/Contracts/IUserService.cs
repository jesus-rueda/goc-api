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
        Task<ICampaingProfile> GetByUpn(string upn);

        Task AutoRegisterUser(string upn);
    }
}
