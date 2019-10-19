using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class TestResultDetail
    {
        public int TestResultDetailId { get; set; }
        public int TestResultId { get; set; }
        public int QuestionId { get; set; }
        public bool IsProperAnswer { get; set; }

        public virtual Question Question { get; set; }
        public virtual TestResult TestResult { get; set; }
    }
}
