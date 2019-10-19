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
    public class TestResultRepository : IRepository<TestResult>
    {
        private ApplicationContext db;

        public TestResultRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<TestResult> GetAll()
        {
            return db.TestResults.Include(o => o.UserProfile);
        }
        public async Task<TestResult> GetAsync(int id)
        {
            return await db.TestResults.FindAsync(id);
        }
        public IEnumerable<TestResult> Find(Func<TestResult, bool> predicate)
        {
            return db.TestResults.Include(o => o.UserProfile).Where(predicate);
        }

        public void Create(TestResult item)
        {
            db.TestResults.Add(item);
        }
        public void Update(TestResult item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            TestResult item = await db.TestResults.FindAsync(id);
            if (item != null)
                db.TestResults.Remove(item);
        }
    }
}
