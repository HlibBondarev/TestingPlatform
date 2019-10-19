using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Models
{
    public class SpecialityViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [ScaffoldColumn(false)]
        public int SubjectId { get; set; }
        [Required]
        [Display(Name = "Speciality name")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }
    }
}