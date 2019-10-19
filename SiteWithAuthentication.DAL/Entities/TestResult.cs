using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class TestResult
    {
        public TestResult()
        {
            this.TestResultDetails = new HashSet<TestResultDetail>();
        }

        public int TestResultId { get; set; }
        public string UserProfileId { get; set; }
        public DateTime TestDate { get; set; }
        public int Result { get; set; }
        public int MaxScore { get; set; }
        public bool IsPassedTest { get; set; }
        public bool IsTopicTest { get; set; }


        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<TestResultDetail> TestResultDetails { get; set; }
    }
}
