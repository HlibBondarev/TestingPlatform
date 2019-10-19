using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class Question
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
            this.TestResultDetails = new HashSet<TestResultDetail>();
        }

        public int QuestionId { get; set; }
        public int TopicId { get; set; }
        public string QuestionText { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public int AnswerTypeId { get; set; }
        public string ResourceRef { get; set; }
        public int QuestionWeight { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<TestResultDetail> TestResultDetails { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual AnswerType AnswerType { get; set; }
    }
}
