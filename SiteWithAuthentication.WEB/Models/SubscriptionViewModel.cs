using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Models
{
    public class SubscriptionViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        [ScaffoldColumn(false)]
        public string UserProfileId { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Incorrect email address")]
        [Display(Name = "User email")]
        public string Email { get; set; }
        [ScaffoldColumn(false)]
        public int CourseId { get; set; }
        [Display(Name = "Course")]
        public int CourseTitle { get; set; }
        [Display(Name = "Start date")]
        public string StartDate { get; set; }
        [Required]
        [Display(Name = "Subscription period")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "366")]
        public int SubscriptionPeriod { get; set; }
        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }
    }
}