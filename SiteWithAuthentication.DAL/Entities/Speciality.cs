using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class Speciality
    {
        public Speciality()
        {
            this.Courses = new HashSet<Course>();
        }

        public int SpecialityId { get; set; }
        //public string UserProfileId { get; set; }
        public int SubjectId { get; set; }
        public string SpecialityName { get; set; }
        public string Description { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public bool IsApproved { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
