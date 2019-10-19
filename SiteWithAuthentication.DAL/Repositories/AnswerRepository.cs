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
    public class AnswerRepository : IRepository<Answer>
    {
        private ApplicationContext db;

        public AnswerRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Answer> GetAll()
        {
            return db.Answers.Include(o => o.Question);
        }
        public async Task<Answer> GetAsync(int id)
        {
            return await db.Answers.FindAsync(id);
        }
        public IEnumerable<Answer> Find(Func<Answer, bool> predicate)
        {
            return db.Answers.Include(o => o.Question).Where(predicate);
        }


        public void Create(Answer item)
        {
            db.Answers.Add(item);
        }
        public void Update(Answer item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Answer item = await db.Answers.FindAsync(id);
            if (item != null)
                db.Answers.Remove(item);
        }
    }
}
