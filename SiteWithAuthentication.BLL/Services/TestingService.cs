using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SiteWithAuthentication.BLL.Util;

namespace SiteWithAuthentication.BLL.Services
{
    class TestingService : ITestingService
    {
        IUnitOfWork Database { get; set; }

        public TestingService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<IList<QuestionDTO>> GetRandomQuestionsForCourse(int courseId)
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                List<Question> source = Database.Question.Find(question => question.Topic.CourseId == courseId).ToList();
                List<QuestionDTO> allCourseQuestions = iMapper.Map<List<Question>, List<QuestionDTO>>(source);
                int questionCount = (await Database.Course.GetAsync(courseId)).CourseTestQuestionsNumber;
                return BLLRepository.RandomQuestionList(allCourseQuestions, questionCount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<QuestionDTO>> GetRandomQuestionsForTopic(int topicId)
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                List<Question> source = Database.Question.Find(question => question.TopicId == topicId).ToList();
                List<QuestionDTO> allTopicQuestions = iMapper.Map<List<Question>, List<QuestionDTO>>(source);
                int courseId = (await Database.Topic.GetAsync(topicId)).CourseId;
                int questionCount = (await Database.Course.GetAsync(courseId)).TopicTestQuestionsNumber;
                return BLLRepository.RandomQuestionList(allTopicQuestions, questionCount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
