using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Models
{
    public class CourseAssignmentViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [ScaffoldColumn(false)]
        public int CourseId { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Incorrect email address")]
        [Display(Name = "User email")]
        public string Email { get; set; }
        [Display(Name = "User name")]
        public string Name { get; set; }
        [Display(Name = "Is approved?")]
        public bool IsApproved { get; set; }
    }
}