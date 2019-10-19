using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class RegistryViewModel
    {
        [Required]
        [Display(Name = "Enter email:")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Incorrect email address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Enter password:")]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,20})", ErrorMessage = "The password must contain 8-20 characters and at least 1 digit, 1 letter in UPPER case, 1 letter in lower case and 1 non-alphanumeric character.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm password:")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Address:")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Name (alias):")]
        public string Name { get; set; }
    }
}