using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class TestDTO
    {
        public int TestId { get; set; }
        public int SubscrimentId { get; set; }
        public DateTime TestDateTime { get; set; }

        public virtual SubscriptionDTO Subscription { get; set; }
        public virtual ICollection<TestResultDTO> TestResults { get; set; }
    }
}
