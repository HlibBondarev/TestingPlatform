using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class UserProfile
    {
        public UserProfile()
        {
            this.Subscriptions = new HashSet<Subscription>();
            this.SubscriptionForModerators = new HashSet<SubscriptionForModerator>();
            this.CourseAssignments = new HashSet<CourseAssignment>();
            this.Courses = new HashSet<Course>();
            this.TestResults = new HashSet<TestResult>();
        }

        [Key]
        [ForeignKey("ApplicationUser")]
        public string UserProfileId { get; set; }
        public string Address { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<SubscriptionForModerator> SubscriptionForModerators { get; set; }
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
