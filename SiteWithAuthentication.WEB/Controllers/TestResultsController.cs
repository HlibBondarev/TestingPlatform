using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Controllers
{
    [Authorize(Roles = "admin, moderator, user")]
    public class TestResultsController : Controller
    {
        private readonly IBLLUnitOfWork bLLUnitOfWork = new BLLUnitOfWork("DefaultConnection");

        #region Services
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        internal ICommonService<TestResultDTO> TestResultService
        {
            get
            {
                return bLLUnitOfWork.TestResultService;
            }
        }
        internal ICommonService<TestResultDetailDTO> TestResultDetailService
        {
            get
            {
                return bLLUnitOfWork.TestResultDetailService;
            }
        }
        internal ICommonService<SubscriptionDTO> SubscriptionService
        {
            get
            {
                return bLLUnitOfWork.SubscriptionService;
            }
        }
        internal ICommonService<CourseDTO> CourseService
        {
            get
            {
                return bLLUnitOfWork.CourseService;
            }
        }
        internal ICommonService<CourseAssignmentDTO> CourseAssignmentService
        {
            get
            {
                return bLLUnitOfWork.CourseAssignmentService;
            }
        }
        #endregion

        #region User test results
        // GET: Index
        public async Task<ActionResult> Index()
        {
            try
            {
                // I. Checks.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle))
                    .ForMember("IsSubscribed", opt => opt.MapFrom(course =>
                                               course.Subscriptions.Where(subscription =>
                                                                          currentUserId != null
                                                                          && subscription.UserProfileId == currentUserId
                                                                          && subscription.CourseId == course.CourseId
                                                                          && (DateTime.Now - subscription.StartDate < TimeSpan.FromDays(subscription.SubscriptionPeriod))
                                                                          && subscription.IsApproved).Count() == 1))
                    .ForMember("IsInApproving", opt => opt.MapFrom(course =>
                                               course.Subscriptions.Where(subscription =>
                                                                          currentUserId != null
                                                                          && subscription.UserProfileId == currentUserId
                                                                          && subscription.CourseId == course.CourseId
                                                                          && (DateTime.Now - subscription.StartDate < TimeSpan.FromDays(subscription.SubscriptionPeriod))
                                                                          && !subscription.IsApproved).Count() == 1));
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle))
                    .ForMember(tr => tr.UserEmail, option => option.Ignore());
                    cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                    .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Set ViewBag properties.
                if (currentUserId != null)
                {
                    ViewBag.UserName = (await UserService.GetAsync(currentUserId)).UserName;
                }
                IEnumerable<TestResultDTO> testResultDTOs = TestResultService.Find(tr =>
                                                                                   tr.UserProfileId == currentUserId
                                                                                   && !tr.IsTopicTest);//.ToList();
                IEnumerable<TestResultViewModel> userTestResults = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(testResultDTOs);
                ViewBag.UserTestResults = userTestResults;

                // IV. Get data for a view.
                IEnumerable<CourseDTO> source = CourseService.Find(dto => dto.IsApproved);//.ToList();
                IEnumerable<CourseViewModel> courses = iMapper.Map<IEnumerable<CourseDTO>, IEnumerable<CourseViewModel>>(source);
                IEnumerable<CourseViewModel> courseOrderedList = courses.Where(course =>
                                                                               course.IsSubscribed
                                                                               || course.IsFree)
                                                                               .OrderBy(course => course.Name)
                                                                               .OrderByDescending(course => course.IsSubscribed);

                // V.
                return View(courseOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: CourseTestResults
        public async Task<ActionResult> CourseTestResults(int? id)
        {
            try
            {
                // I. Checks.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get CourseDTO object.
                CourseDTO courseDTO = await CourseService.GetAsync(intId);
                if (courseDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.UserName = (await UserService.GetAsync(currentUserId)).UserName;
                ViewBag.CourseTitle = courseDTO.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle))
                    .ForMember(tr => tr.TestResultDetails, option => option.Ignore())
                    .ForMember(tr => tr.UserEmail, option => option.Ignore());
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a View.
                IEnumerable<TestResultDTO> source = TestResultService.Find(tr =>
                                                                           tr.UserProfileId == currentUserId
                                                                           && !tr.IsTopicTest);//.ToList();
                IEnumerable<TestResultViewModel> courseTestResultList = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(source)
                                                                        .Where(tr => tr.TestTitle == courseDTO.CourseTitle)
                                                                        .OrderBy(tr => tr.TestTitle);

                // V.
                Session["PreviousAction"] = "CourseTestResults";
                Session["UserEmail"] = null;
                return View(courseTestResultList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // GET: CourseTestResults
        public async Task<ActionResult> TopicTestResults(int? id)
        {
            try
            {
                // I. Checks.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.UserName = (await UserService.GetAsync(currentUserId)).UserName;
                // Get CourseDTO object.
                CourseDTO courseDTO = await CourseService.GetAsync(intId);
                ViewBag.CourseTitle = courseDTO.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.TopicTitle))
                    .ForMember(tr => tr.TestResultDetails, option => option.Ignore())
                    .ForMember(tr => tr.UserEmail, option => option.Ignore());
                });
                IMapper iMapper = config.CreateMapper();

                //IV.Get data for a View.
                IEnumerable<TestResultDTO> temp = TestResultService.Find(tr =>
                                                                           tr.UserProfileId == currentUserId
                                                                           && tr.IsTopicTest);//.ToList();
                IEnumerable<TestResultDTO> source = temp.Where(tr =>
                                                               tr.TestResultDetails.FirstOrDefault().Question.Topic.CourseId == intId).ToList();
                IEnumerable<TestResultViewModel> testResultList = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(source).OrderBy(tr => tr.TestTitle);

                // V.
                Session["PreviousAction"] = "TopicTestResults";
                Session["UserEmail"] = null;
                return View(testResultList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Course test result details list.
        public async Task<ActionResult> CourseTestResultDetails(int? id)
        {
            try
            {
                // I.Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get TestResultDTO object.
                TestResultDTO testResultDTO = await TestResultService.GetAsync(intId);
                if (testResultDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle))
                    .ForMember(tr => tr.UserEmail, option => option.Ignore());
                    cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                    .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Set ViewBag properties.
                TestResultViewModel testResult = iMapper.Map<TestResultDTO, TestResultViewModel>(testResultDTO);
                ViewBag.CourseName = testResult.TestTitle;
                ViewBag.Result = testResult.Result * 1000 / testResult.MaxScore;
                ViewBag.MaxScore = 1000;
                ViewBag.ParentId = testResultDTO.TestResultDetails.FirstOrDefault().Question.Topic.CourseId;

                // IV. Get data for a view.
                IEnumerable<TestResultDetailDTO> source = TestResultDetailService.Find(trd => trd.TestResultId == intId);//.ToList();
                IEnumerable<TestResultDetailViewModel> testResultDetailList = iMapper.Map<IEnumerable<TestResultDetailDTO>, IEnumerable<TestResultDetailViewModel>>(source).OrderBy(trd => trd.Topic);

                // V.
                return View(testResultDetailList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Topic test result details list.
        public async Task<ActionResult> TopicTestResultDetails(int? id)
        {
            try
            {
                // I.Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get TestResultDTO object.
                TestResultDTO testResultDTO = await TestResultService.GetAsync(intId);
                if (testResultDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.TopicTitle))
                    .ForMember(tr => tr.UserEmail, option => option.Ignore());
                    cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                    .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Set ViewBag properties.
                TestResultViewModel testResult = iMapper.Map<TestResultDTO, TestResultViewModel>(testResultDTO);
                ViewBag.CourseName = testResultDTO.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle;
                ViewBag.TopicName = testResultDTO.TestResultDetails.FirstOrDefault().Question.Topic.TopicTitle;
                ViewBag.Result = testResult.Result * 1000 / testResult.MaxScore;
                ViewBag.MaxScore = 1000;
                ViewBag.ParentId = testResultDTO.TestResultDetails.FirstOrDefault().Question.Topic.CourseId;

                // IV. Get data for a view.
                IEnumerable<TestResultDetailDTO> source = TestResultDetailService.Find(trd => trd.TestResultId == intId);//.ToList();
                IEnumerable<TestResultDetailViewModel> testResultDetailList = iMapper.Map<IEnumerable<TestResultDetailDTO>, IEnumerable<TestResultDetailViewModel>>(source).OrderBy(trd => trd.Topic);

                // V.
                return View(testResultDetailList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Subscriber test results
        // GET: ModeratorCourses (moderator course list)
        [Authorize(Roles = "admin, moderator")]
        public ActionResult ModeratorCourses()
        {
            try
            {
                // I. Check.
                // Get Id for the current user.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Get data for a view.
                IEnumerable<CourseDTO> source = CourseAssignmentService.Find(ca => ca.UserProfileId == currentUserId
                                                                                   && ca.IsApproved)
                                                                       .Select(ca => ca.Course)
                                                                       .OrderBy(ca => ca.CourseTitle);//.ToList();
                IEnumerable<CourseViewModel> userAssignmentCourses = iMapper.Map<IEnumerable<CourseDTO>, IEnumerable<CourseViewModel>>(source);

                // IV.
                return View(userAssignmentCourses);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: ModeratorCourseSubscribers (moderator course subscriber list)
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> ModeratorCourseSubscribers(int? id)
        {
            // I. Checks.
            //  Get Id for the selected course.
            if (!int.TryParse(id.ToString(), out int intId))
            {
                return RedirectToAction("Index");
            }
            // Get CourseDTO object.
            CourseDTO courseDTO = await CourseService.GetAsync(intId);
            if (courseDTO == null)
            {
                return RedirectToAction("Index");
            }

            // II. Set ViewBag properties.
            ViewBag.CourseName = courseDTO.CourseTitle;
            ViewBag.CourseId = intId;

            // III. AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle))
                    .ForMember("UserEmail", opt => opt.MapFrom(tr => tr.UserProfile.Email));
                cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
            });
            IMapper iMapper = config.CreateMapper();

            // IV. Get data for a view.
            IEnumerable<SubscriptionDTO> courseSubscriptions = SubscriptionService.Find(dto =>
                                                               dto.CourseId == intId
                                                               && dto.IsApproved);
            //IEnumerable<TestResultDTO> source = TestResultService.Find(tr => !tr.IsTopicTest)
            IEnumerable<TestResultDTO> source = TestResultService.GetAll()
                                                                 .Where(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.CourseId == intId)
                                                                 .Join(courseSubscriptions,
                                                                       tr => tr.UserProfileId,
                                                                       cs => cs.UserProfileId,
                                                                       (tr, cs) => tr).Distinct();//.ToList();
            IEnumerable<TestResultViewModel> courseTestResults = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(source);
            IEnumerable<SubscriberTestResultViewModel> resultView = courseTestResults.GroupBy(ctr => ctr.UserEmail)
                                                       .Select(ctr => new SubscriberTestResultViewModel
                                                       {
                                                           UserEmail = ctr.Key,
                                                           AttemptsNumber = ctr.Count(item => !item.IsTopicTest),
                                                           BestResult = ctr.Max((item) =>
                                                           {
                                                               if (!item.IsTopicTest)
                                                               {
                                                                   return item.Result * 1000 / item.MaxScore;
                                                               }
                                                               return 0;
                                                           }),
                                                           IsPassedTest = ctr.Max((item) =>
                                                           {
                                                               if (!item.IsTopicTest)
                                                               {
                                                                   return item.IsPassedTest;
                                                               }
                                                               return false;
                                                           })
                                                       });

            // V.
            return View(resultView);
        }

        // GET: Moderator course test results (course results list)
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> ModeratorCourseTestResults(int? courseId, string userEmail)
        {
            try
            {
                // I. Checks.
                //  Get course Id for the selected course and check User email.
                if (!int.TryParse(courseId.ToString(), out int intCourseId) && userEmail != null)
                {
                    return RedirectToAction("Index");
                }
                // Get CourseDTO object.
                CourseDTO courseDTO = await CourseService.GetAsync(intCourseId);
                if (courseDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.CourseTitle = courseDTO.CourseTitle;
                ViewBag.ParentId = intCourseId;
                ViewBag.Trainee = userEmail;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                        .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                        .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle))
                        .ForMember("UserEmail", opt => opt.MapFrom(tr => tr.UserProfile.Email));
                    cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                    .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IEnumerable<SubscriptionDTO> courseSubscriptions = SubscriptionService.Find(dto =>
                                                                   dto.CourseId == intCourseId
                                                                   && dto.Email == userEmail);
                IEnumerable<TestResultDTO> source = TestResultService.Find(tr => !tr.IsTopicTest)
                                                                     .Where(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.CourseId == intCourseId)
                                                                     .Join(courseSubscriptions,
                                                                           tr => tr.UserProfileId,
                                                                           cs => cs.UserProfileId,
                                                                           (tr, cs) => tr).Distinct();//.ToList();
                IEnumerable<TestResultViewModel> courseTestResults = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(source);

                // V.
                Session["PreviousAction"] = "ModeratorCourseTestResults";
                Session["UserEmail"] = userEmail;
                return View(courseTestResults);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: ModeratorTopicTestResults
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> ModeratorTopicTestResults(int? courseId, string userEmail)
        {
            try
            {
                // I. Checks.
                if (userEmail == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                string userId = UserService.FindUserIdByEmail(userEmail);
                // Check id.
                if (!int.TryParse(courseId.ToString(), out int intCourseId))
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.UserName = (await UserService.GetAsync(userId)).UserName;
                // Get CourseDTO object.
                CourseDTO courseDTO = await CourseService.GetAsync(intCourseId);
                ViewBag.CourseTitle = courseDTO.CourseTitle;
                ViewBag.ParentId = intCourseId;
                ViewBag.Trainee = userEmail;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.TopicTitle))
                    .ForMember(tr => tr.TestResultDetails, option => option.Ignore())
                    .ForMember(tr => tr.UserEmail, option => option.Ignore());
                });
                IMapper iMapper = config.CreateMapper();

                //IV.Get data for a View.
                IEnumerable<TestResultDTO> testResultDTOs = TestResultService.Find(tr => tr.UserProfileId == userId && tr.IsTopicTest);
                IEnumerable<TestResultDTO> source = TestResultService.Find(tr => tr.UserProfileId == userId && tr.IsTopicTest)
                                                                     .Where(course => course.TestResultDetails.FirstOrDefault().Question.Topic.CourseId == intCourseId);
                //.ToList();
                IEnumerable<TestResultViewModel> testResults = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(source).OrderBy(tr => tr.TestTitle);

                // V.
                Session["PreviousAction"] = "ModeratorTopicTestResults";
                Session["UserEmail"] = userEmail;
                return View(testResults);
            }
            catch (Exception ex)
            {
                string innerException = string.Empty;
                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }
                throw new Exception("Error!!!       InnerException: " + innerException + ";    " + ex.Message);
            }
        }
        #endregion
    }
}