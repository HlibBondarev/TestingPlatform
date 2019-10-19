using System;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.DTO;

namespace SiteWithAuthentication.BLL.Interfaces
{
    public interface IBLLUnitOfWork : IDisposable
    {
        IUserService UserService { get; }
        ITestingService TestingService { get; }


        Task SaveAsync();


        // Others...
        ICommonService<SubjectDTO> SubjectService { get; }
        ICommonService<SpecialityDTO> SpecialityService { get; }
        ICommonService<CourseDTO> CourseService { get; }
        ICommonService<CourseAssignmentDTO> CourseAssignmentService { get; }
        ICommonService<TopicDTO> TopicService { get; }
        ICommonService<QuestionDTO> QuestionService { get; }
        ICommonService<AnswerDTO> AnswerService { get; }
        ICommonService<SubscriptionForModeratorDTO> SubscriptionForModeratorService { get; }
        ICommonService<SubscriptionDTO> SubscriptionService { get; }
        ICommonService<TestResultDTO> TestResultService { get; }
        ICommonService<TestResultDetailDTO> TestResultDetailService { get; }
    }
}
