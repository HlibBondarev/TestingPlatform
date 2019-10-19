using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Models
{
    public class SubjectViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Subject name")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }
    }
}