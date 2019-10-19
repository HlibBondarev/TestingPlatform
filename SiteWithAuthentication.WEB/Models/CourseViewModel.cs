using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Models
{
    public class CourseViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [ScaffoldColumn(false)]
        public int SpecialityId { get; set; }
        [Required]
        [Display(Name = "Course title")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Number of questions in the course test")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "100")]
        public int CourseTestQuestionsNumber { get; set; }
        [Required]
        [Display(Name = "Number of questions in the topic test")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "10")]
        public int TopicTestQuestionsNumber { get; set; }
        [Required]
        [Display(Name = "Time to answer one question")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "600")]
        public int TimeToAnswerOneQuestion { get; set; }
        [Required]
        [Display(Name = "Number of attempts")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "100")]
        public int AttemptsNumber { get; set; }
        [Required]
        [Display(Name = "Passing score")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "100")]
        public int PassingScore { get; set; }
        [Required]
        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }
        [Required]
        [Display(Name = "Is free")]
        public bool IsFree { get; set; }

        public bool IsSubscribed { get; set; }
        public bool IsInApproving { get; set; }
    }
}