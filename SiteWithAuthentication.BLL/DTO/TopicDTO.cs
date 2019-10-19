using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class TopicDTO
    {
        public int TopicId { get; set; }
        public int CourseId { get; set; }
        public string TopicTitle { get; set; }
        public int TopicNumber { get; set; }
        public string Description { get; set; }
        public bool IsFree { get; set; }

        public virtual ICollection<QuestionDTO> Questions { get; set; }
        public virtual CourseDTO Course { get; set; }
    }
}
