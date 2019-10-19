using System;

namespace SiteWithAuthentication.DAL.Entities
{
    public class SubscriptionForModerator
    {
        public int SubscriptionForModeratorId { get; set; }
        public string UserProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public int SubscriptionPeriod { get; set; }
        public int CourseCount { get; set; }
        public bool IsTrial { get; set; }
        public bool IsApproved { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}

