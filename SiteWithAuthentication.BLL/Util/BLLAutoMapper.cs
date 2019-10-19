using AutoMapper;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.DAL.Entities;

namespace SiteWithAuthentication.BLL.Util
{
    public static class BLLAutoMapper
    {
        // Static ctor.
        static BLLAutoMapper()
        {
            object lockObject = new object();
            lock (lockObject)
            {
                // AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserProfile, UserProfileDTO>()
                        .ForMember("Id", opt => opt.MapFrom(obj => obj.UserProfileId))
                        .ForMember("Address", opt => opt.MapFrom(obj => obj.Address))
                        .ForMember("UserName", opt => opt.MapFrom(obj => obj.ApplicationUser.UserName))
                        .ForMember("Password", opt => opt.MapFrom(obj => obj.ApplicationUser.PasswordHash))
                        .ForMember("Email", opt => opt.MapFrom(obj => obj.ApplicationUser.Email))
                        .ForMember("EmailConfirmed", opt => opt.MapFrom(obj => obj.ApplicationUser.EmailConfirmed))
                        .ForMember("PhoneNumber", opt => opt.MapFrom(obj => obj.ApplicationUser.PhoneNumber))
                        .ForMember("PhoneNumberConfirmed", opt => opt.MapFrom(obj => obj.ApplicationUser.PhoneNumberConfirmed))
                        .ForMember(c => c.Role, option => option.Ignore());
                    cfg.CreateMap<ApplicationUser, UserProfile>()
                        .ForMember("UserProfileId", opt => opt.MapFrom(obj => obj.Id))
                        .ForMember(c => c.Address, option => option.Ignore())
                        .ForMember("ApplicationUser", opt => opt.MapFrom(obj => obj));
                    cfg.CreateMap<Subject, SubjectDTO>();
                    cfg.CreateMap<Speciality, SpecialityDTO>();
                    cfg.CreateMap<Course, CourseDTO>();
                    cfg.CreateMap<Subscription, SubscriptionDTO>()
                        .ForMember("Email", opt => opt.MapFrom(obj => obj.UserProfile.ApplicationUser.Email));
                    cfg.CreateMap<CourseAssignment, CourseAssignmentDTO>()
                       .ForMember("Email", opt => opt.MapFrom(obj => obj.UserProfile.ApplicationUser.Email))
                       .ForMember("Name", opt => opt.MapFrom(obj => obj.UserProfile.ApplicationUser.UserName));
                    cfg.CreateMap<Topic, TopicDTO>();
                    cfg.CreateMap<Question, QuestionDTO>();
                    cfg.CreateMap<AnswerType, AnswerTypeDTO>();
                    cfg.CreateMap<Answer, AnswerDTO>();
                    cfg.CreateMap<SubscriptionForModerator, SubscriptionForModeratorDTO>()
                       .ForMember("Email", opt => opt.MapFrom(obj => obj.UserProfile.ApplicationUser.Email));
                    cfg.CreateMap<TestResult, TestResultDTO>();
                    cfg.CreateMap<TestResultDetail, TestResultDetailDTO>();
                });
                GetMapper = config.CreateMapper();
                //throw new Exception("Hi!!!");
            }
        }

        // Static property.
        public static IMapper GetMapper { get; private set; }
    }
}
