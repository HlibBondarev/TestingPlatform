using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SiteWithAuthentication.DAL.Entities;


namespace SiteWithAuthentication.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(string conectionString) : base(conectionString) { }

        // This default Constructor for migrations!!!
        //public ApplicationContext() : base("DefaultConnection") { }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    throw new UnintentionalCodeFirstException();
        //}

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AnswerType> AnswerTypes { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseAssignment> CourseAssignments { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<SubscriptionForModerator> SubscriptionForModerators { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<TestResultDetail> TestResultDetails { get; set; }

        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
    }
}
