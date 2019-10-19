using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class AnswerType
    {
        public AnswerType()
        {
            this.Questions = new HashSet<Question>();
        }
        public int AnswerTypeId { get; set; }
        public string AnswerTypeDescription { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
