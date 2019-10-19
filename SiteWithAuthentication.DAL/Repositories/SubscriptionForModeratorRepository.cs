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
    public class SubscriptionForModeratorRepository : IRepository<SubscriptionForModerator>
    {
        private ApplicationContext db;

        public SubscriptionForModeratorRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<SubscriptionForModerator> GetAll()
        {
            return db.SubscriptionForModerators.Include(u => u.UserProfile);
        }
        public async Task<SubscriptionForModerator> GetAsync(int id)
        {
            return await db.SubscriptionForModerators.FindAsync(id);
        }
        public IEnumerable<SubscriptionForModerator> Find(Func<SubscriptionForModerator, bool> predicate)
        {
            return db.SubscriptionForModerators.Include(u => u.UserProfile).Where(predicate);
        }

        public void Create(SubscriptionForModerator item)
        {
            db.SubscriptionForModerators.Add(item);
        }
        public void Update(SubscriptionForModerator item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            SubscriptionForModerator item = await db.SubscriptionForModerators.FindAsync(id);
            if (item != null)
                db.SubscriptionForModerators.Remove(item);
        }
    }
}