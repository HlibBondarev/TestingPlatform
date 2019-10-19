using System;

namespace SiteWithAuthentication.BLL.DTO
{
    public class CourseAssignmentDTO
    {
        public int CourseAssignmentId { get; set; }
        public string UserProfileId { get; set; }
        public int CourseId { get; set; }
        public System.DateTime AssignmentDate { get; set; }
        public bool IsApproved { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public virtual CourseDTO Course { get; set; }
        public virtual UserProfileDTO UserProfile { get; set; }
    }
}
