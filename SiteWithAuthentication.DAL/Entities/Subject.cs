using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteWithAuthentication.DAL.Entities
{
    public class Subject
    {
        public Subject()
        {
            this.Specialities = new HashSet<Speciality>();
        }

        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }

        public DateTime LastModifiedDateTime { get; set; }
        public bool IsApproved { get; set; }

        public virtual ICollection<Speciality> Specialities { get; set; }
    }
}
