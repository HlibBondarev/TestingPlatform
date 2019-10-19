using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public string UserProfileId { get; set; }
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public int SubscriptionPeriod { get; set; }
        public bool IsApproved { get; set; }
        public string Email { get; set; }

        public virtual CourseDTO Course { get; set; }
        public virtual UserProfileDTO UserProfile { get; set; }
    }
}
