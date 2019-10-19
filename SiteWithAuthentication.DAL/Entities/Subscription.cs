using System;
using System.Collections.Generic;


namespace SiteWithAuthentication.DAL.Entities
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public string UserProfileId { get; set; }
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public int SubscriptionPeriod { get; set; }
        public bool IsApproved { get; set; }

        public virtual Course Course { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}