using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class UserProfileDTO
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public virtual ICollection<SubscriptionDTO> Subscriptions { get; set; }
        public virtual ICollection<CourseDTO> Courses { get; set; }
        public virtual ICollection<CourseAssignmentDTO> CourseAssignments { get; set; }
        public virtual ICollection<TestResultDTO> TestResults { get; set; }
    }
}
