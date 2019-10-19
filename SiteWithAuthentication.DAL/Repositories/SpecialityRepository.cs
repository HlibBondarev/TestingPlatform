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
    public class SpecialityRepository : IRepository<Speciality>
    {
        private ApplicationContext db;

        public SpecialityRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Speciality> GetAll()
        {
            return db.Specialities.Include(o => o.Subject);
        }
        public async Task<Speciality> GetAsync(int id)
        {
            return await db.Specialities.FindAsync(id);
        }
        public IEnumerable<Speciality> Find(Func<Speciality, bool> predicate)
        {
            return db.Specialities.Include(o => o.Subject).Where(predicate);
        }

        public void Create(Speciality item)
        {
            db.Specialities.Add(item);
        }
        public void Update(Speciality item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            Speciality item = await db.Specialities.FindAsync(id);
            if (item != null)
                db.Specialities.Remove(item);
        }
    }
}
