using System;

namespace SiteWithAuthentication.DAL.Entities
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsProper { get; set; }
        public DateTime LastModifiedDateTime { get; set; }

        public virtual Question Question { get; set; }
    }
}
