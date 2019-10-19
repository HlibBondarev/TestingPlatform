using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Enter email:")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Incorrect email address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Enter password:")]
        public string Password { get; set; }
    }
}