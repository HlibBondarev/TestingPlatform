using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Models
{
    public class ModeratorSubscriptionViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        [ScaffoldColumn(false)]
        public string UserProfileId { get; set; }
        [Display(Name = "User email")]
        public string Email { get; set; }
        [Display(Name = "Start date")]
        public string StartDate { get; set; }
        [Required]
        [Display(Name = "Subscription period")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "366")]
        public int SubscriptionPeriod { get; set; }
        [Required]
        [Display(Name = "Course count")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "100")]
        public int CourseCount { get; set; }
        [Display(Name = "Is trial")]
        public bool IsTrial { get; set; }
        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }
    }
}