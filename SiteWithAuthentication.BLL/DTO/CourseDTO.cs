using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public int SpecialityId { get; set; }
        public string UserProfileId { get; set; }
        public string CourseTitle { get; set; }
        public int CourseTestQuestionsNumber { get; set; }
        public int TopicTestQuestionsNumber { get; set; }
        public int TimeToAnswerOneQuestion { get; set; }
        public string Description { get; set; }
        public int AttemptsNumber { get; set; }
        public int PassingScore { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFree { get; set; }

        public virtual ICollection<SubscriptionDTO> Subscriptions { get; set; }
        public virtual ICollection<TopicDTO> Topics { get; set; }
        public virtual ICollection<CourseAssignmentDTO> CourseAssignments { get; set; }
        public virtual SpecialityDTO Speciality { get; set; }
        public virtual UserProfileDTO UserProfiles { get; set; }
    }
}
