using System;

namespace SiteWithAuthentication.DAL.Entities
{
    public class CourseAssignment
    {
        public int CourseAssignmentId { get; set; }
        public string UserProfileId { get; set; }
        public int CourseId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public bool IsApproved { get; set; }

        public virtual Course Course { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
