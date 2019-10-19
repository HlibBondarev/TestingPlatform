using System;
using System.ComponentModel.DataAnnotations;

namespace SiteWithAuthentication.WEB.Models
{
    public class AnswerViewModel
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string Answer { get; set; }
        [Required]
        public bool IsProper { get; set; }
    }
}