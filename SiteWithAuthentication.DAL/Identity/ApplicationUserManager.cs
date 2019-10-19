using SiteWithAuthentication.DAL.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
        {
        }

        public IEnumerable<ApplicationUser> Find(Func<ApplicationUser, bool> converted)
        {
            throw new NotImplementedException();
        }
    }
}
