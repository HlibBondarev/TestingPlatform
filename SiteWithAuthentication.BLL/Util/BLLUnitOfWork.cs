using System;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Services;
using SiteWithAuthentication.DAL.Repositories;

namespace SiteWithAuthentication.BLL.Util
{
    public class BLLUnitOfWork : IBLLUnitOfWork
    {
        // Application context variable.
        private EFUnitOfWork uof;
        // Private service variables.
        private UserService userService;
        private TestingService testingService;
        private CourseService courseService;
        private CourseAssignmentService courseAssignmentService;
        private SpecialityService specialityService;
        private SubjectService subjectService;
        private TopicService topicService;
        private QuestionService questionService;
        private AnswerService answerService;
        private SubscriptionForModeratorService subscriptionForModeratorService;
        private SubscriptionService subscriptionService;
        private TestResultService testResultService;
        private TestResultDetailService testResultDetailService;

        // Constructor.
        public BLLUnitOfWork(string connectionString)
        {
            uof = new EFUnitOfWork(connectionString);
        }

        // Properties.
        public IUserService UserService
        {
            get
            {
                if (userService == null)
                    userService = new UserService(uof);
                return userService;
            }
        }
        public ITestingService TestingService
        {
            get
            {
                if (testingService == null)
                    testingService = new TestingService(uof);
                return testingService;
            }
        }
        public ICommonService<SubjectDTO> SubjectService
        {
            get
            {
                if (subjectService == null)
                    subjectService = new SubjectService(uof);
                return subjectService;
            }
        }
        public ICommonService<SpecialityDTO> SpecialityService
        {
            get
            {
                if (specialityService == null)
                    specialityService = new SpecialityService(uof);
                return specialityService;
            }
        }
        public ICommonService<CourseDTO> CourseService
        {
            get
            {
                if (courseService == null)
                    courseService = new CourseService(uof);
                return courseService;
            }
        }
        public ICommonService<CourseAssignmentDTO> CourseAssignmentService
        {
            get
            {
                if (courseAssignmentService == null)
                    courseAssignmentService = new CourseAssignmentService(uof);
                return courseAssignmentService;
            }
        }
        public ICommonService<TopicDTO> TopicService
        {
            get
            {
                if (topicService == null)
                    topicService = new TopicService(uof);
                return topicService;
            }
        }
        public ICommonService<QuestionDTO> QuestionService
        {
            get
            {
                if (questionService == null)
                    questionService = new QuestionService(uof);
                return questionService;
            }
        }
        public ICommonService<AnswerDTO> AnswerService
        {
            get
            {
                if (answerService == null)
                    answerService = new AnswerService(uof);
                return answerService;
            }
        }
        public ICommonService<SubscriptionForModeratorDTO> SubscriptionForModeratorService
        {
            get
            {
                if (subscriptionForModeratorService == null)
                    subscriptionForModeratorService = new SubscriptionForModeratorService(uof);
                return subscriptionForModeratorService;
            }
        }
        public ICommonService<SubscriptionDTO> SubscriptionService
        {
            get
            {
                if (subscriptionService == null)
                    subscriptionService = new SubscriptionService(uof);
                return subscriptionService;
            }
        }
        public ICommonService<TestResultDTO> TestResultService
        {
            get
            {
                if (testResultService == null)
                    testResultService = new TestResultService(uof);
                return testResultService;
            }
        }
        public ICommonService<TestResultDetailDTO> TestResultDetailService
        {
            get
            {
                if (testResultDetailService == null)
                    testResultDetailService = new TestResultDetailService(uof);
                return testResultDetailService;
            }
        }

        // Realisation of interface (IUnitOfWork) methods - SaveAsync() and Dispose().
        public async Task SaveAsync()
        {
            await uof.SaveAsync();
        }
        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    uof.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
