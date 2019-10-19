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
    public class SubjectRepository : IRepository<Subject>
    {
        private ApplicationContext db;

        public SubjectRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Subject> GetAll()
        {
            return db.Subjects;
        }
        public async Task<Subject> GetAsync(int id)
        {
            return await db.Subjects.FindAsync(id);
        }
        public IEnumerable<Subject> Find(Func<Subject, bool> predicate)
        {
            try
            {
                return db.Subjects.Where(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Create(Subject item)
        {
            db.Subjects.Add(item);
        }
        public void Update(Subject item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Subject item = await db.Subjects.FindAsync(id);
            if (item != null)
                db.Subjects.Remove(item);
        }
    }
}
