using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public partial class Course
    {
        public Course()
        {
            this.Subscriptions = new HashSet<Subscription>();
            this.Topics = new HashSet<Topic>();
            this.CourseAssignments = new HashSet<CourseAssignment>();
        }

        public int CourseId { get; set; }
        public int SpecialityId { get; set; }
        public string UserProfileId { get; set; }
        public string CourseTitle { get; set; }
        public int CourseTestQuestionsNumber { get; set; }
        public int TopicTestQuestionsNumber { get; set; }
        public int TimeToAnswerOneQuestion { get; set; }
        public string Description { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public int AttemptsNumber { get; set; }
        public int PassingScore { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFree { get; set; }

        public virtual Speciality Speciality { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; }
    }
}
