using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class TestResultDTO
    {
        public int TestResultId { get; set; }
        public string UserProfileId { get; set; }
        public DateTime TestDate { get; set; }
        public int Result { get; set; }
        public int MaxScore { get; set; }
        public bool IsPassedTest { get; set; }
        public bool IsTopicTest { get; set; }

        public virtual UserProfileDTO UserProfile { get; set; }
        public virtual ICollection<TestResultDetailDTO> TestResultDetails { get; set; }
    }
}
