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
    public class AnswerTypeRepository : IRepository<AnswerType>
    {
        private ApplicationContext db;

        public AnswerTypeRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<AnswerType> GetAll()
        {
            return db.AnswerTypes;
        }
        public async Task<AnswerType> GetAsync(int id)
        {
            return await db.AnswerTypes.FindAsync(id);
        }
        public IEnumerable<AnswerType> Find(Func<AnswerType, bool> predicate)
        {
            return db.AnswerTypes.Where(predicate);
        }

        public void Create(AnswerType item)
        {
            db.AnswerTypes.Add(item);
        }
        public void Update(AnswerType item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            AnswerType item = await db.AnswerTypes.FindAsync(id);
            if (item != null)
                db.AnswerTypes.Remove(item);
        }
    }
}
