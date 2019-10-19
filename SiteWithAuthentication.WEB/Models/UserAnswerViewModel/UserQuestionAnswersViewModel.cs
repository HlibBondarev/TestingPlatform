using System.Collections.Generic;

namespace SiteWithAuthentication.WEB.Models.UserAnswerViewModel
{
    public class UserQuestionAnswersViewModel
    {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string ResourceRef { get; set; }
        public string AnswerType { get; set; }
        public int QuestionWeight { get; set; }


        public IEnumerable<UserAnswer> AvailableUserAnswers { get; set; }
        public IEnumerable<UserAnswer> SelectedUserAnswers { get; set; }
        public PostedUserAnswers PostedUserAnswers { get; set; }
    }
}