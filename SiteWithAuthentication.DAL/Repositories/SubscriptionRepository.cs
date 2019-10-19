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
    public class SubscriptionRepository : IRepository<Subscription>
    {
        private ApplicationContext db;

        public SubscriptionRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Subscription> GetAll()
        {
            return db.Subscriptions.Include(c => c.Course).Include(u => u.UserProfile);
        }
        public async Task<Subscription> GetAsync(int id)
        {
            return await db.Subscriptions.FindAsync(id);
        }
        public IEnumerable<Subscription> Find(Func<Subscription, bool> predicate)
        {
            return db.Subscriptions.Include(c => c.Course).Include(u => u.UserProfile).Where(predicate);
        }

        public void Create(Subscription item)
        {
            db.Subscriptions.Add(item);
        }
        public void Update(Subscription item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Subscription item = await db.Subscriptions.FindAsync(id);
            if (item != null)
                db.Subscriptions.Remove(item);
        }
    }
}
