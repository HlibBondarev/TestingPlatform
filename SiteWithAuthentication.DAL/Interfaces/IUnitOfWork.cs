using System;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Identity;
using System.Threading.Tasks;

namespace SiteWithAuthentication.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Answer> Answer { get; }
        IRepository<AnswerType> AnswerType { get; }
        IRepository<Course> Course { get; }
        IRepository<CourseAssignment> CourseAssignment { get; }
        IRepository<Question> Question { get; }
        IRepository<Speciality> Speciality { get; }
        IRepository<Subject> Subject { get; }
        IRepository<Subscription> Subscription { get; }
        IRepository<SubscriptionForModerator> SubscriptionForModerator { get; }
        IRepository<TestResult> TestResult { get; }
        IRepository<TestResultDetail> TestResultDetail { get; }
        IRepository<Topic> Topic { get; }
        IRepository<UserProfile> UserProfile { get; }

        ApplicationUserManager UserManager { get; }
        ApplicationRoleManager RoleManager { get; }

        Task SaveAsync();
    }
}
