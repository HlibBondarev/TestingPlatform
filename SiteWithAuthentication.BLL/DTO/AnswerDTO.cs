using System;

namespace SiteWithAuthentication.BLL.DTO
{
    public class AnswerDTO
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsProper { get; set; }

        public virtual QuestionDTO Question { get; set; }
    }
}
