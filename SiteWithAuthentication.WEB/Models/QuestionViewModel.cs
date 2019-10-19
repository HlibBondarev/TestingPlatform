using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class QuestionViewModel
    {
        public int ID { get; set; }
        public int TopicId { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string Question { get; set; }
        [Display(Name = "Answer type")]
        public string AnswerType { get; set; }
        [Required]
        public int AnswerTypeId { get; set; }
        [DataType(DataType.ImageUrl)]
        [UIHint("Url")]
        [Display(Name = "Resource reference")]
        public string ResourceRef { get; set; }
        [Display(Name = "Weight")]
        [Range(typeof(int), "1", "100")]
        public int QuestionWeight { get; set; }
    }
}