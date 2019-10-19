using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteWithAuthentication.WEB.Models
{
    public class TestResultDetailViewModel
    {
        public int QuestionId { get; set; }
        public bool IsProperAnswer { get; set; }

        public string Question { get; set; }
        public string Topic { get; set; }
    }
}