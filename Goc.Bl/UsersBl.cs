using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business
{
    using Goc.Business.Contracts;
    using Goc.Models;
    using Microsoft.EntityFrameworkCore;

    internal class UsersBl: IUserBl
    {
        private readonly GocContext myContext;
        public UsersBl(GocContext context)
        {
            this.myContext = context;
        }


        public async Task<User> GetByUpn(string upn)
        {
            return await this.myContext.Users.Where(x => x.Upn == upn).FirstOrDefaultAsync();
        }

        public async Task AutoRegisterUser(string upn)
        { 
            var user = await this.GetByUpn(upn); // hate this
            if (user == null)
            {
                user = new User() { Upn = upn };
                await this.myContext.Users.AddAsync(user);
                await this.myContext.SaveChangesAsync();
            }
            Console.WriteLine(user);
        }
    }
}
