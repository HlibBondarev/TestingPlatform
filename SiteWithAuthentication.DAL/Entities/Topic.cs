using System;
using System.Collections.Generic;

namespace SiteWithAuthentication.DAL.Entities
{
    public class Topic
    {
        public Topic()
        {
            this.Questions = new HashSet<Question>();
        }

        public int TopicId { get; set; }
        public int CourseId { get; set; }
        public string TopicTitle { get; set; }
        public int TopicNumber { get; set; }
        public string Description { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public bool IsFree { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual Course Course { get; set; }
    }
}
