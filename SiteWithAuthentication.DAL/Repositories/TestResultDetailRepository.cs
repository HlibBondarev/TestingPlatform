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
    public class TestResultDetailRepository : IRepository<TestResultDetail>
    {
        private ApplicationContext db;

        public TestResultDetailRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<TestResultDetail> GetAll()
        {
            return db.TestResultDetails.Include(o => o.Question).Include(o => o.TestResult);
        }
        public async Task<TestResultDetail> GetAsync(int id)
        {
            return await db.TestResultDetails.FindAsync(id);
        }
        public IEnumerable<TestResultDetail> Find(Func<TestResultDetail, bool> predicate)
        {
            return db.TestResultDetails.Include(o => o.Question).Include(o => o.TestResult).Where(predicate);
        }

        public void Create(TestResultDetail item)
        {
            db.TestResultDetails.Add(item);
        }
        public void Update(TestResultDetail item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            TestResultDetail item = await db.TestResultDetails.FindAsync(id);
            if (item != null)
                db.TestResultDetails.Remove(item);
        }
    }
}
