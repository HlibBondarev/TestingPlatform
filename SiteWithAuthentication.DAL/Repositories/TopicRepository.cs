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
    public class TopicRepository : IRepository<Topic>
    {
        private ApplicationContext db;

        public TopicRepository(ApplicationContext context)
        {
            this.db = context;
        }
        public IEnumerable<Topic> GetAll()
        {
            return db.Topics.Include(o => o.Course);
        }
        public async Task<Topic> GetAsync(int id)
        {
            return await db.Topics.FindAsync(id);
        }
        public IEnumerable<Topic> Find(Func<Topic, bool> predicate)
        {
            return db.Topics.Include(o => o.Course).Where(predicate);
        }

        public void Create(Topic item)
        {
            db.Topics.Add(item);
        }
        public void Update(Topic item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Topic item = await db.Topics.FindAsync(id);
            if (item != null)
                db.Topics.Remove(item);
        }
    }
}
