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
    public class CourseAssignmentRepository : IRepository<CourseAssignment>
    {
        private ApplicationContext db;

        public CourseAssignmentRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<CourseAssignment> GetAll()
        {
            return db.CourseAssignments.Include(c => c.Course).Include(u => u.UserProfile);
        }
        public async Task<CourseAssignment> GetAsync(int id)
        {
            return await db.CourseAssignments.FindAsync(id);
        }
        public IEnumerable<CourseAssignment> Find(Func<CourseAssignment, bool> predicate)
        {
            return db.CourseAssignments.Include(c => c.Course).Include(u => u.UserProfile).Where(predicate);
        }


        public void Create(CourseAssignment item)
        {
            db.CourseAssignments.Add(item);
        }
        public void Update(CourseAssignment item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            CourseAssignment item = await db.CourseAssignments.FindAsync(id);
            if (item != null)
                db.CourseAssignments.Remove(item);
        }
    }
}
