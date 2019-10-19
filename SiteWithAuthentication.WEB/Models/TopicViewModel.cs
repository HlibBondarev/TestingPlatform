using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class TopicViewModel
    {
        public int ID { get; set; }
        public int CourseId { get; set; }
        [Required]
        [Display(Name = "Topic title")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Number")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "100")]
        public int TopicNumber { get; set; }
        [Required]
        [Display(Name = "Is free")]
        public bool IsFree { get; set; }
    }
}