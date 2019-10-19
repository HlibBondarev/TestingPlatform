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
    public class CourseRepository : IRepository<Course>
    {
        private ApplicationContext db;

        public CourseRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Course> GetAll()
        {
            return db.Courses.Include(o => o.Speciality).Include(o =>o.UserProfile);
        }
        public async Task<Course> GetAsync(int id)
        {
            return await db.Courses.FindAsync(id);
        }
        public IEnumerable<Course> Find(Func<Course, bool> predicate)
        {
            return db.Courses.Include(o => o.Speciality).Include(o => o.UserProfile).Where(predicate);
        }

        public void Create(Course item)
        {
            db.Courses.Add(item);
        }
        public void Update(Course item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Course item = await db.Courses.FindAsync(id);
            if (item != null)
                db.Courses.Remove(item);
        }
    }
}
