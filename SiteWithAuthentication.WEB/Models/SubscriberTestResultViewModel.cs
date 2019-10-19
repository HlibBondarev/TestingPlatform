using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class SubscriberTestResultViewModel
    {
        [Display(Name = "User email")]
        public string UserEmail { get; set; }
        [Display(Name = "Attempts number")]
        public int AttemptsNumber { get; set; }
        [Display(Name = "Best result")]
        public int BestResult { get; set; }
        [Display(Name = "Is passed")]
        public bool IsPassedTest { get; set; }
    }
}