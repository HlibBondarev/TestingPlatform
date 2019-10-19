using System;
using System.Threading.Tasks;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.EF;
using SiteWithAuthentication.DAL.Interfaces;
using SiteWithAuthentication.DAL.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SiteWithAuthentication.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        // Application context variable.
        private ApplicationContext db;
        // Private repository variables.
        private AnswerRepository answerRepository;
        private AnswerTypeRepository answerTypeRepository;
        private CourseAssignmentRepository courseAssignmentRepository;
        private CourseRepository courseRepository;
        private QuestionRepository questionRepository;
        private SpecialityRepository specialityRepository;
        private SubjectRepository subjectRepository;
        private SubscriptionRepository subscriptionRepository;
        private SubscriptionForModeratorRepository subscriptionForModeratorRepository;
        private TestResultRepository testResultRepository;
        private TestResultDetailRepository testResultDetailRepository;
        private TopicRepository topicRepository;
        private UserProfileRepository userProfileRepository;
        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;

        // Constructor.
        public EFUnitOfWork(string connectionString)
        {
            db = new ApplicationContext(connectionString);
        }

        // Properties.
        public IRepository<Answer> Answer
        {
            get
            {
                if (answerRepository == null)
                    answerRepository = new AnswerRepository(db);
                return answerRepository;
            }
        }
        public IRepository<AnswerType> AnswerType
        {
            get
            {
                if (answerTypeRepository == null)
                    answerTypeRepository = new AnswerTypeRepository(db);
                return answerTypeRepository;
            }
        }
        public IRepository<Course> Course
        {
            get
            {
                if (courseRepository == null)
                    courseRepository = new CourseRepository(db);
                return courseRepository;
            }
        }
        public IRepository<CourseAssignment> CourseAssignment
        {
            get
            {
                if (courseAssignmentRepository == null)
                    courseAssignmentRepository = new CourseAssignmentRepository(db);
                return courseAssignmentRepository;
            }
        }
        public IRepository<Question> Question
        {
            get
            {
                if (questionRepository == null)
                    questionRepository = new QuestionRepository(db);
                return questionRepository;
            }
        }
        public IRepository<Speciality> Speciality
        {
            get
            {
                if (specialityRepository == null)
                    specialityRepository = new SpecialityRepository(db);
                return specialityRepository;
            }
        }
        public IRepository<Subject> Subject
        {
            get
            {
                if (subjectRepository == null)
                    subjectRepository = new SubjectRepository(db);
                return subjectRepository;
            }
        }

        public IRepository<SubscriptionForModerator> SubscriptionForModerator
        {
            get
            {
                if (subscriptionForModeratorRepository == null)
                    subscriptionForModeratorRepository = new SubscriptionForModeratorRepository(db);
                return subscriptionForModeratorRepository;
            }
        }
        public IRepository<Subscription> Subscription
        {
            get
            {
                if (subscriptionRepository == null)
                    subscriptionRepository = new SubscriptionRepository(db);
                return subscriptionRepository;
            }
        }
        public IRepository<TestResult> TestResult
        {
            get
            {
                if (testResultRepository == null)
                    testResultRepository = new TestResultRepository(db);
                return testResultRepository;
            }
        }
        public IRepository<TestResultDetail> TestResultDetail
        {
            get
            {
                if (testResultDetailRepository == null)
                    testResultDetailRepository = new TestResultDetailRepository(db);
                return testResultDetailRepository;
            }
        }
        public IRepository<Topic> Topic
        {
            get
            {
                if (topicRepository == null)
                    topicRepository = new TopicRepository(db);
                return topicRepository;
            }
        }
        public IRepository<UserProfile> UserProfile
        {
            get
            {
                if (userProfileRepository == null)
                    userProfileRepository = new UserProfileRepository(db);
                return userProfileRepository;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (userManager == null)
                    userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                return userManager;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                if (roleManager == null)
                    roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
                return roleManager;
            }
        }

        // Realisation of interface (IUnitOfWork) methods - SaveAsync() and Dispose().
        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
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
