using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class TestResultViewModel
    {
        public TestResultViewModel()
        {
            this.TestResultDetails = new HashSet<TestResultDetailViewModel>();
        }

        public int Result { get; set; }
        public int MaxScore { get; set; }
        [Display(Name = "Is passed")]
        public bool IsPassedTest { get; set; }
        public int Id { get; set; }
        [Display(Name = "Title")]
        public string TestTitle { get; set; }
        public string UserEmail { get; set; }
        public bool IsTopicTest { get; set; }

        public virtual ICollection<TestResultDetailViewModel> TestResultDetails { get; set; }
    }
}