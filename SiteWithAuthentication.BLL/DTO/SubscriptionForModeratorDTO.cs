using System;

namespace SiteWithAuthentication.BLL.DTO
{
    public class SubscriptionForModeratorDTO
    {
        public int SubscriptionForModeratorId { get; set; }
        public string UserProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public int SubscriptionPeriod { get; set; }
        public int CourseCount { get; set; }
        public bool IsTrial { get; set; }
        public bool IsApproved { get; set; }
        public string Email { get; set; }


        public virtual UserProfileDTO UserProfile { get; set; }
    }
}

