using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public int TopicId { get; set; }
        public string QuestionText { get; set; }
        public int AnswerTypeId { get; set; }
        public string ResourceRef { get; set; }
        public int QuestionWeight { get; set; }

        public virtual TopicDTO Topic { get; set; }
        public virtual AnswerTypeDTO AnswerType { get; set; }
        public virtual ICollection<AnswerDTO> Answers { get; set; }
        public virtual ICollection<TestResultDetailDTO> TestResultDetails { get; set; }
    }
}
