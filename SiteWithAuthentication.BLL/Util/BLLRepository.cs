using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteWithAuthentication.BLL.Util
{
    public static class BLLRepository
    {
        public static bool IsAdmin(IUnitOfWork Database, string userId)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                string roleId = Database.RoleManager.Roles.FirstOrDefault(obj => obj.Name == "admin").Id;
                ApplicationUser user = Database.UserManager.Users.FirstOrDefault(obj => obj.Id == userId);
                var isAdmin = (from userRole in user.Roles
                               where userRole.RoleId == roleId
                               select userRole).Count() > 0;
                return isAdmin;
            }
        }

        public static bool IsModerator(IUnitOfWork Database, string userId)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                string roleId = Database.RoleManager.Roles.FirstOrDefault(obj => obj.Name == "moderator").Id;
                ApplicationUser user = Database.UserManager.Users.FirstOrDefault(obj => obj.Id == userId);
                var isModerator = (from userRole in user.Roles
                               where userRole.RoleId == roleId
                               select userRole).Count() > 0;
                return isModerator;
            }
        }

        public static bool CheckCountOfProperAnswers(string answerType, bool isProperAnswer,
                                                     int countOfExistedProperAnswersInQuestion,
                                                     out string result)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                bool isCorrect = true;
                result = null;
                switch (answerType)
                {
                    case "Text":
                        if (countOfExistedProperAnswersInQuestion > 0 || !isProperAnswer)
                        {
                            result = "The question with type answer - 'Text' can contains only one answer and it must be proper!";
                            isCorrect = false;
                        }
                        break;

                    case "RadioButton":
                        if (countOfExistedProperAnswersInQuestion > 0 && isProperAnswer)
                        {
                            result = "The question with type answer - 'RadioButton' can contains only one proper answer!";
                            isCorrect = false;
                        }
                        break;
                }
                return isCorrect;
            }
        }


        // Returns a list of numberCount random integer numbers less than maxValue.
        static List<int> RandomIntList(int maxValue, int numberCount)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                Random staticRandom = new Random();
                List<int> randomList = new List<int>();
                while (randomList.Count < numberCount)
                {
                    int newNumber = staticRandom.Next(0, maxValue);
                    if (!randomList.Contains(newNumber))
                        randomList.Add(newNumber);
                }
                return randomList;
            }
        }

        // Returns a list of questionCount random questions.
        public static List<QuestionDTO> RandomQuestionList(List<QuestionDTO> list, int questionCount)
        {
            object lockObj = new object();
            lock (lockObj)
            {
                if (list.Count <= questionCount) return list;
                List<int> randomIntList = RandomIntList(list.Count, questionCount);
                List<QuestionDTO> randomQuetiopnList = new List<QuestionDTO>();
                for (int i = 0; i < questionCount; i++)
                {
                    randomQuetiopnList.Add(list[randomIntList[i]]);
                }
                return randomQuetiopnList;
            }
        }
    }
}
