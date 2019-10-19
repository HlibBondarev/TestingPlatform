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
    public class QuestionRepository : IRepository<Question>
    {
        private ApplicationContext db;

        public QuestionRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Question> GetAll()
        {
            return db.Questions.Include(o => o.Topic);
        }
        public async Task<Question> GetAsync(int id)
        {
            return await db.Questions.FindAsync(id);
        }
        public IEnumerable<Question> Find(Func<Question, bool> predicate)
        {
            return db.Questions.Include(o => o.Topic).Where(predicate);
        }

        public void Create(Question item)
        {
            db.Questions.Add(item);
        }
        public void Update(Question item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Question item = await db.Questions.FindAsync(id);
            if (item != null)
                db.Questions.Remove(item);
        }
    }
}
