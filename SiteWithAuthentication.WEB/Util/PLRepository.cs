using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.WEB.Models.UserAnswerViewModel;
using SiteWithAuthentication.WEB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SiteWithAuthentication.WEB.Util
{
    public static class PLRepository
    {
        internal static bool IsValidImage(string fileName, int fileLength)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                // Valid extensions: *.gif, *.png и *.jpg.
                List<string> extensions = new List<string>() { ".gif", ".png", ".jpg" };

                // ASP has a download limit of 4096 Kb. 
                // In order to increase the restriction in web.config you need to add the line < httpRuntime maxRequestLength = "20480" />
                int maxLength = 1048576; // The maximum file size: 1 Mb (1 * 1024 * 1024)
                // Get the file extension.
                string fileExtension = Path.GetExtension(fileName);
                // Check the file extension.
                if (!extensions.Contains(fileExtension))
                {
                    return false;
                }
                // Check the file length.
                if (fileLength > maxLength)
                {
                    return false;
                }
                return true;
            }
        }

        internal static string CreateNewFileName(int fileNumber)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                // Form a name for the new file.
                return DateTime.Now.Year.ToString() + "-"
                                    + DateTime.Now.Month.ToString() + "-"
                                    + DateTime.Now.Day.ToString() + "-"
                                    + DateTime.Now.Hour.ToString() + "-"
                                    + DateTime.Now.Minute.ToString() + "-"
                                    + DateTime.Now.Second.ToString() + "-"
                                    + fileNumber.ToString();
            }
        }

        internal static void SetViewBagProperiesForModeratorSubscription(IUserService userService, string currentUserId,
                                               ICommonService<SubscriptionForModeratorDTO> subscriptionForModeratorService,
                                               out bool? viewBagIsAllowedToSuggest,
                                               out bool? viewBagGoToTrialSubscription,
                                               out bool? viewBagGoToSubscription)
        {
            // Initialize properties of ViewBag object.
            viewBagIsAllowedToSuggest = false;
            viewBagGoToTrialSubscription = false;
            viewBagGoToSubscription = null;

            // If a current user is Admin.
            if (userService.FindUserRoleById(currentUserId).ToLower() == "admin")
            {
                viewBagIsAllowedToSuggest = true;
                return;
            }

            // Get all subscriptions of the current user.
            IEnumerable<SubscriptionForModeratorDTO> subscriptions = subscriptionForModeratorService.Find(obj => obj.UserProfileId == currentUserId);
            // Checking for: Has the current user already had a trial subscription?
            bool IsAlreadyHadTrialSubscription = (from subscription in subscriptions
                                                  where subscription.IsTrial
                                                  select subscription).Count() > 0;
            // Get active subscriptions.
            List<SubscriptionForModeratorDTO> activeSubscriptions =
                (from subscription in subscriptions
                 where subscription.StartDate < DateTime.Now
                 && (DateTime.Now - subscription.StartDate < TimeSpan.FromDays(subscription.SubscriptionPeriod))
                 select subscription).ToList();
            // I. If there isn't a single active subscriptions:
            if (activeSubscriptions.Count() == 0)
            {
                if (!IsAlreadyHadTrialSubscription)
                {
                    viewBagGoToTrialSubscription = true;
                }
                viewBagGoToSubscription = true;
            }
            // II. If there is only one active subscription:
            else if (activeSubscriptions.Count() == 1)
            {
                if (activeSubscriptions.First().IsTrial)
                {
                    viewBagGoToSubscription = true;
                }
                else
                {
                    if (activeSubscriptions.First().IsApproved)
                    {
                        viewBagIsAllowedToSuggest = true;
                    }
                    else
                    {
                        viewBagGoToSubscription = false;
                    }
                }
            }
            // III. If there are two active subscriptions one of them must have a trial status:
            else if (activeSubscriptions.Count() == 2 && (activeSubscriptions[0].IsTrial || activeSubscriptions[1].IsTrial))
            {
                viewBagGoToSubscription = true;
                viewBagGoToSubscription = false;
            }
            else
            {
                throw new Exception("There can be a maximum of two active subscriptions! And one of them must have a trial status!");
            }
        }


        // Check!!!
        internal static TestResultViewModel CalculateTestResults(Dictionary<int, List<UserAnswer>> allAnswers,
                                                               Dictionary<int, List<UserAnswer>> userAnswers,
                                                               List<UserQuestionAnswersViewModel> questionList)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                try
                {
                    // I. Check.
                    if (allAnswers.Count == 0 || userAnswers.Count == 0)
                    {
                        throw new Exception("Parameters allAnswers and (or) userAnswers is empty!!!");
                    }

                    // II. Get the test results and set them to testResult var .
                    TestResultViewModel testResult = new TestResultViewModel();
                    TestResultDetailViewModel testResultDetail;
                    int testScore = 0;
                    int questionWeight = 0;
                    bool isProperAnswer = false;
                    int maxScore = 0;
                    foreach (var item in allAnswers)
                    {
                        isProperAnswer = IsProperAnswer(questionList.Find(question => question.QuestionId == item.Key).AnswerType,
                                                        userAnswers[item.Key],
                                                        allAnswers[item.Key]);
                        testResultDetail = new TestResultDetailViewModel()
                        {
                            QuestionId = item.Key,
                            IsProperAnswer = isProperAnswer
                        };
                        questionWeight = questionList.Find(question => question.QuestionId == item.Key).QuestionWeight;
                        if (isProperAnswer)
                        {
                            testScore += questionWeight;
                        }
                        maxScore += questionWeight;
                        testResult.TestResultDetails.Add(testResultDetail);
                    }
                    testResult.MaxScore = maxScore;
                    testResult.Result = testScore;
                    return testResult;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private static bool IsProperAnswer(string answerType, List<UserAnswer> userAnswerList, List<UserAnswer> properAnswerList)
        {
            try
            {
                //List<UserAnswer> selectedUserAnswerList = userAnswerList.Where(answer => answer.IsSelected = true).ToList();
                UserAnswer firstUserAnswer = userAnswerList.Where(answer => answer.IsSelected = true).ToList().FirstOrDefault();
                if (firstUserAnswer == null)
                {
                    return false;
                }
                List<UserAnswer> selectedProperAnswerList = properAnswerList.Where(answer => answer.IsSelected == true).ToList();
                UserAnswer firstProperAnswer = selectedProperAnswerList.FirstOrDefault();
                if (firstProperAnswer == null)
                {
                    throw new Exception("There isn't any one answer marked like proper!!! This is the test moderator error.");
                }

                switch (answerType.ToLower())
                {
                    case "checkbox":
                        if (userAnswerList.Count != selectedProperAnswerList.Count)
                        {
                            return false;
                        }
                        for (int i = 0, length = userAnswerList.Count; i < length; i++)
                        {
                            if (!selectedProperAnswerList.Find(answer => answer.Id == userAnswerList[i].Id).IsSelected)
                            {
                                return false;
                            }
                        }
                        return true;

                    case "text":
                        if (firstUserAnswer.Answer == firstProperAnswer.Answer)
                            return true;
                        break;

                    case "radiobutton":
                        if (firstUserAnswer.Id == firstProperAnswer.Id)
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
