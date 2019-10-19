using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class TestResultDetailDTO
    {
        public int TestResultDetailId { get; set; }
        public int TestResultId { get; set; }
        public int QuestionId { get; set; }
        public bool IsProperAnswer { get; set; }

        public virtual QuestionDTO Question { get; set; }
        public virtual TestResultDTO TestResult { get; set; }
    }
}
