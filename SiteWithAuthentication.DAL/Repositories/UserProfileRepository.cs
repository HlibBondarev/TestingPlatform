using System;
using System.Collections.Generic;
using System.Linq;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.EF;
using SiteWithAuthentication.DAL.Interfaces;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SiteWithAuthentication.DAL.Repositories
{
    public class UserProfileRepository : IRepository<UserProfile>
    {
        private ApplicationContext db;

        public UserProfileRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<UserProfile> GetAll()
        {
            return db.UserProfiles;
        }
        public async Task<UserProfile> GetAsync(int id)
        {
            return await db.UserProfiles.FindAsync(id);
        }

        // Unimplemented method.
        public IEnumerable<UserProfile> Find(Func<UserProfile, bool> predicate)
        {
            throw new NotImplementedException();
            //return db.UserProfiles.Where(predicate);
        }

        public void Create(UserProfile item)
        {
            db.UserProfiles.Add(item);
        }
        public void Update(UserProfile item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            UserProfile item = await db.UserProfiles.FindAsync(id);
            if (item != null)
                db.UserProfiles.Remove(item);
        }
    }
}
