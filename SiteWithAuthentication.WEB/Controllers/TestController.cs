using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.WEB.Models;
using SiteWithAuthentication.WEB.Models.UserAnswerViewModel;
using SiteWithAuthentication.WEB.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SiteWithAuthentication.WEB.Controllers
{
    public class TestController : Controller
    {
        private readonly IBLLUnitOfWork bLLUnitOfWork = new BLLUnitOfWork("DefaultConnection");

        #region Properties for test processing. 
        // Property to save "questions - answers" dictionary from DB.
        private Dictionary<int, List<UserAnswer>> AllAnswers
        {
            get
            {
                object val = Session["AllAnswers"];
                if (val != null)
                {
                    return (Dictionary<int, List<UserAnswer>>)val;
                }
                Session["AllAnswers"] = new Dictionary<int, List<UserAnswer>>();
                return new Dictionary<int, List<UserAnswer>>();
            }
            set { Session["AllAnswers"] = value; }
        }
        // Property to save the user answers in "questions - answers" dictionary.
        public Dictionary<int, List<UserAnswer>> UserAnswers
        {
            get
            {
                object val = Session["UserAnswers"];
                if (val != null)
                {
                    return (Dictionary<int, List<UserAnswer>>)val;
                }
                Session["UserAnswers"] = new Dictionary<int, List<UserAnswer>>();
                return new Dictionary<int, List<UserAnswer>>();
            }
            set { Session["UserAnswers"] = value; }
        }
        // Property to save the test questions in List<QuestionViewModel>.
        public List<UserQuestionAnswersViewModel> TestQuetions
        {
            get
            {
                object val = Session["TestQuetions"];
                if (val != null)
                {
                    return (List<UserQuestionAnswersViewModel>)val;
                }
                Session["TestQuetions"] = new List<UserQuestionAnswersViewModel>();
                return new List<UserQuestionAnswersViewModel>();
            }
            set { Session["TestQuetions"] = value; }
        }
        #endregion

        #region Properties for processing a test period. 
        // Test duration properties and methods.
        private int TestPeriod
        {
            get
            {
                object temp = Session["TestPeriod"];
                if (temp != null && int.TryParse(temp.ToString(), out int val))
                {
                    return val;
                }
                return 0;
            }
            set { Session["TestPeriod"] = value; }
        }
        private DateTime? StartTestTime
        {
            get
            {
                object temp = Session["StartTestTime"];
                if (temp != null && DateTime.TryParse(temp.ToString(), out DateTime val))
                {
                    return val;
                }
                return null;
            }
            set
            {
                object temp = Session["StartTestTime"];
                if (value == null || temp == null)
                {
                    Session["StartTestTime"] = value;
                }
            }
        }
        private int LeftTestPeriod
        {
            get
            {
                object temp = Session["StartTestTime"];
                if (temp != null && DateTime.TryParse(temp.ToString(), out DateTime val))
                {
                    return TestPeriod - (int)(DateTime.Now - val).TotalSeconds;
                }
                Session["TestPeriod"] = null;
                return 0;
            }
        }
        #endregion

        #region Services
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        private ITestingService TestingService
        {
            get
            {
                return bLLUnitOfWork.TestingService;
            }
        }
        private ICommonService<SubjectDTO> SubjectService
        {
            get
            {
                return bLLUnitOfWork.SubjectService;
            }
        }
        private ICommonService<SpecialityDTO> SpecialityService
        {
            get
            {
                return bLLUnitOfWork.SpecialityService;
            }
        }
        private ICommonService<CourseDTO> CourseService
        {
            get
            {
                return bLLUnitOfWork.CourseService;
            }
        }
        private ICommonService<TopicDTO> TopicService
        {
            get
            {
                return bLLUnitOfWork.TopicService;
            }
        }
        private ICommonService<QuestionDTO> QuestionService
        {
            get
            {
                return bLLUnitOfWork.QuestionService;
            }
        }
        private ICommonService<AnswerDTO> AnswerService
        {
            get
            {
                return bLLUnitOfWork.AnswerService;
            }
        }
        internal ICommonService<SubscriptionDTO> SubscriptionService
        {
            get
            {
                return bLLUnitOfWork.SubscriptionService;
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
        #endregion

        #region Navigation and Subscribe actions.
        // GET: Test/Index        
        public ActionResult Index()
        {
            try
            {
                // Clear properties for processing a test period.
                TestPeriod = 0;
                StartTestTime = null;

                // I. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SubjectDTO, SubjectViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.SubjectId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.SubjectName));
                });
                IMapper iMapper = config.CreateMapper();

                // II. Get data for a view.
                IEnumerable<SubjectViewModel> subjects = iMapper.Map<IEnumerable<SubjectDTO>, IEnumerable<SubjectViewModel>>(SubjectService.Find(sub => sub.IsApproved == true))
                                                                .OrderBy(obj => obj.Name);

                //// Clear Session["TestPeriod"] which keeps timer period.
                //Session["TestPeriod"] = null;

                // III.
                return View(subjects);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Subject specialities list.
        public async Task<ActionResult> SubjectSpecialities(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get subject.
                SubjectDTO subject = await SubjectService.GetAsync(intId);
                if (subject == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = intId;
                ViewBag.SubjectName = subject.SubjectName;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SpecialityDTO, SpecialityViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.SpecialityId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.SpecialityName));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                List<SpecialityDTO> source = SpecialityService.Find(dto => dto.SubjectId == intId && dto.IsApproved == true).OrderBy(o => o.SpecialityName).ToList();
                IEnumerable<SpecialityViewModel> specialityOrderedList = iMapper.Map<IEnumerable<SpecialityDTO>, IEnumerable<SpecialityViewModel>>(source);

                // V.
                return View(specialityOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Speciality courses list.
        public async Task<ActionResult> SpecialityCourses(int? id)
        {
            try
            {
                // I.Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get SpecialityDTO object.
                SpecialityDTO specialityDTO = await SpecialityService.GetAsync(intId);
                if (specialityDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. AutoMapper Setup.
                // Get Id for the current user.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
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
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.Course.CourseTitle));
                    cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                    .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Set ViewBag properties.
                // Get a role name for the current user.
                if (currentUserId != null)
                    ViewBag.RoleName = UserService.FindUserRoleById(currentUserId);
                else
                    ViewBag.RoleName = null;
                ViewBag.ParentId = intId;
                ViewBag.SpecialityName = specialityDTO.SpecialityName;
                ViewBag.ParentParentId = specialityDTO.SubjectId;
                IEnumerable<TestResultDTO> testResultDTOs = TestResultService.Find(tr =>
                                                                                   tr.UserProfileId == currentUserId
                                                                                   && !tr.IsTopicTest);
                IEnumerable<TestResultViewModel> userTestResults = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(testResultDTOs);
                ViewBag.UserTestResults = userTestResults;

                // IV. Get data for a view.
                IEnumerable<CourseDTO> source = CourseService.Find(dto => dto.SpecialityId == intId && dto.IsApproved)
                    .OrderBy(o => o.CourseTitle).OrderByDescending(o => o.IsFree);
                IEnumerable<CourseViewModel> courseOrderedList = iMapper.Map<IEnumerable<CourseDTO>, IEnumerable<CourseViewModel>>(source);

                // V.
                return View(courseOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Course topics list.
        public async Task<ActionResult> CourseTopics(int? id)
        {
            try
            {
                // I.Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get courseDTO object.
                CourseDTO courseDTO = await CourseService.GetAsync(intId);
                if (courseDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TestResultDTO, TestResultViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(tr => tr.TestResultId))
                    .ForMember("TestTitle", opt => opt.MapFrom(tr => tr.TestResultDetails.FirstOrDefault().Question.Topic.TopicTitle));
                    cfg.CreateMap<TestResultDetailDTO, TestResultDetailViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(trd => trd.Question.QuestionText))
                    .ForMember("Topic", opt => opt.MapFrom(trd => trd.Question.Topic.TopicTitle));
                    cfg.CreateMap<TopicDTO, TopicViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.TopicId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Set ViewBag properties.
                // Get a role name for the current user.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId != null)
                    ViewBag.RoleName = UserService.FindUserRoleById(currentUserId);
                else
                    ViewBag.RoleName = null;
                // Check: is this user subscribed to this course?
                SubscriptionDTO activeSubscription = SubscriptionService.Find(obj =>
                                                obj.UserProfileId == currentUserId
                                                && obj.CourseId == intId
                                                && (DateTime.Now - obj.StartDate < TimeSpan.FromDays(obj.SubscriptionPeriod)))
                                                .FirstOrDefault();
                ViewBag.IsInApproving = false;
                ViewBag.IsSubscribed = false;
                if (activeSubscription != null)
                {
                    if (activeSubscription.IsApproved)
                    {
                        ViewBag.IsSubscribed = true;
                    }
                    else
                    {
                        ViewBag.IsInApproving = true;
                    }
                }
                ViewBag.ParentId = intId;
                ViewBag.CourseName = courseDTO.CourseTitle;
                ViewBag.ParentParentId = courseDTO.SpecialityId;
                IEnumerable<TestResultDTO> testResultDTOs = TestResultService.Find(tr => tr.UserProfileId == currentUserId
                                                                                         && tr.IsTopicTest);
                IEnumerable<TestResultViewModel> userTestResults = iMapper.Map<IEnumerable<TestResultDTO>, IEnumerable<TestResultViewModel>>(testResultDTOs);
                ViewBag.UserTestResults = userTestResults;
                ViewBag.AttemptsNumber = courseDTO.AttemptsNumber;

                // IV. Get data for a view.
                List<TopicDTO> source = TopicService.Find(dto => dto.CourseId == intId).OrderBy(o => o.TopicNumber).ToList();
                IEnumerable<TopicViewModel> topicOrderedList = iMapper.Map<IEnumerable<TopicDTO>, IEnumerable<TopicViewModel>>(source);

                // VI.
                return View(topicOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Test/Subscribe
        public async Task<ActionResult> Subscribe(int? id)
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

                // II. Add a new subscription to DB.
                if (ModelState.IsValid)
                {
                    SubscriptionDTO subscriptionDTO = new SubscriptionDTO
                    {
                        UserProfileId = currentUserId,
                        CourseId = intId,
                        SubscriptionPeriod = 1,
                        IsApproved = false
                    };
                    OperationDetails operationDetails = await SubscriptionService.CreateAsync(subscriptionDTO, currentUserId);
                    if (operationDetails.Succedeed)
                    {
                        return PartialView("Report", operationDetails);
                    }
                    else
                    {
                        ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
                        return PartialView("Report", operationDetails);
                    }
                }
                ViewBag.Message = "Non valid";
                return PartialView();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        // Start a course test.
        public async Task<ActionResult> CourseTest(int? id)
        {
            try
            {
                // I.Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get courseDTO object.
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
                    cfg.CreateMap<QuestionDTO, UserQuestionAnswersViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(obj => obj.QuestionText))
                    .ForMember("AnswerType", opt => opt.MapFrom(obj => obj.AnswerType.AnswerTypeDescription))
                    .ForMember("AvailableUserAnswers", opt => opt.MapFrom(obj => obj.Answers))
                    .ForMember("QuestionWeight", opt => opt.MapFrom(obj => obj.QuestionWeight))
                    .ForMember(q => q.SelectedUserAnswers, option => option.Ignore())
                    .ForMember(q => q.PostedUserAnswers, option => option.Ignore());
                    cfg.CreateMap<AnswerDTO, UserAnswer>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.AnswerId))
                    .ForMember("Answer", opt => opt.MapFrom(obj => obj.AnswerText))
                    .ForMember("IsSelected", opt => opt.MapFrom(obj => obj.IsProper));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IList<QuestionDTO> source = await TestingService.GetRandomQuestionsForCourse(intId);
                IEnumerable<UserQuestionAnswersViewModel> courseTestQuestionList = iMapper.Map<IEnumerable<QuestionDTO>, IEnumerable<UserQuestionAnswersViewModel>>(source);

                // V. Set properties: TestQuetions, AllAnswers and UserAnswers.
                TestQuetions = courseTestQuestionList.ToList();
                AllAnswers.Clear();
                UserAnswers.Clear();
                foreach (var item in courseTestQuestionList)
                {
                    AllAnswers[item.QuestionId] = item.AvailableUserAnswers.ToList();
                    UserAnswers[item.QuestionId] = new List<UserAnswer>();
                }

                // VI. Set Timer.
                TestPeriod = courseDTO.TimeToAnswerOneQuestion * courseTestQuestionList.Count();
                StartTestTime = DateTime.Now;

                // VII.
                return View(courseTestQuestionList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Method gets the remaining test period.
        [HttpPost]
        public JsonResult GetRemainingTestPeriod()
        {
            try
            {
                var jsonData = new
                {
                    period = LeftTestPeriod
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                return Json(string.Format("Exeption: {0}\n Exception in Json formating!", ex.Message));
            }
        }

        // Start a topic test.
        public async Task<ActionResult> TopicTest(int? id)
        {
            try
            {
                // I.Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get courseDTO object.
                TopicDTO topicDTO = await TopicService.GetAsync(intId);
                if (topicDTO == null)
                {
                    return RedirectToAction("Index");
                }
                //Get courseDTO object.
                CourseDTO courseDTO = await CourseService.GetAsync(topicDTO.CourseId);
                if (courseDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.CourseName = courseDTO.CourseTitle;
                ViewBag.TopicName = topicDTO.TopicTitle;
                ViewBag.TopicId = intId;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<QuestionDTO, UserQuestionAnswersViewModel>()
                    .ForMember("Question", opt => opt.MapFrom(obj => obj.QuestionText))
                    .ForMember("AnswerType", opt => opt.MapFrom(obj => obj.AnswerType.AnswerTypeDescription))
                    .ForMember("AvailableUserAnswers", opt => opt.MapFrom(obj => obj.Answers))
                    .ForMember("QuestionWeight", opt => opt.MapFrom(obj => obj.QuestionWeight))
                    .ForMember(q => q.SelectedUserAnswers, option => option.Ignore())
                    .ForMember(q => q.PostedUserAnswers, option => option.Ignore());
                    cfg.CreateMap<AnswerDTO, UserAnswer>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.AnswerId))
                    .ForMember("Answer", opt => opt.MapFrom(obj => obj.AnswerText))
                    .ForMember("IsSelected", opt => opt.MapFrom(obj => obj.IsProper));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IList<QuestionDTO> source = await TestingService.GetRandomQuestionsForTopic(intId);
                IEnumerable<UserQuestionAnswersViewModel> topicTestQuestionList = iMapper.Map<IEnumerable<QuestionDTO>, IEnumerable<UserQuestionAnswersViewModel>>(source);

                // V. Set properties: TestQuetions, AllAnswers and UserAnswers.
                TestQuetions = topicTestQuestionList.ToList();
                AllAnswers.Clear();
                UserAnswers.Clear();
                foreach (var item in topicTestQuestionList)
                {
                    AllAnswers[item.QuestionId] = item.AvailableUserAnswers.ToList();
                    UserAnswers[item.QuestionId] = new List<UserAnswer>();
                }

                // VI. Set Timer.
                TestPeriod = courseDTO.TimeToAnswerOneQuestion * topicTestQuestionList.Count();
                StartTestTime = DateTime.Now;

                // VII.
                return View(topicTestQuestionList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Partial views methods for user answers.
        // Methods for CheckBox answers.
        [HttpGet]
        public ActionResult CheckBoxQuestionAnswers(int? id)
        {
            try
            {
                // I. Check.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }

                // II. Get data for a view.
                // Get question.
                UserQuestionAnswersViewModel question = TestQuetions.Find(q => q.QuestionId == intId);
                if (question == null)
                {
                    return RedirectToAction("Index");
                }
                question.SelectedUserAnswers = UserAnswers[intId];

                return PartialView(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckBoxQuestionAnswers(PostedUserAnswers postedUserAnswers, int QuestionId)
        {
            UserAnswer userAnswer;
            try
            {
                UserAnswers[QuestionId].Clear();
                if (postedUserAnswers.AnswerIds != null)
                {
                    for (int i = 0, length = postedUserAnswers.AnswerIds.Count(); i < length; i++)
                    {
                        if (!int.TryParse(postedUserAnswers.AnswerIds[i], out int answerId))
                        {
                            throw new Exception("Answer Id doesn't cast to integer type");
                        }
                        userAnswer = TestQuetions.Find(q => q.QuestionId == QuestionId).AvailableUserAnswers.Where(a => a.Id == answerId).FirstOrDefault();
                        userAnswer.IsSelected = true;
                        UserAnswers[QuestionId].Add(userAnswer);
                    }
                }
                UserQuestionAnswersViewModel question = TestQuetions.Find(q => q.QuestionId == QuestionId);
                question.SelectedUserAnswers = UserAnswers[QuestionId];
                return PartialView(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Methods for TextBox answers.
        [HttpGet]
        public ActionResult TextQuestionAnswers(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }

                // II. Get data for a view.
                // Get question.
                UserQuestionAnswersViewModel question = TestQuetions.Find(q => q.QuestionId == intId);
                if (question == null)
                {
                    return RedirectToAction("Index");
                }
                if (UserAnswers[intId].Count == 0)
                {
                    UserAnswer userAnswer = new UserAnswer()
                    {
                        Id = question.AvailableUserAnswers.FirstOrDefault().Id,
                        IsSelected = true,
                        Answer = string.Empty
                    };
                    UserAnswers[intId].Add(userAnswer);
                }
                question.SelectedUserAnswers = UserAnswers[intId];

                return PartialView(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TextQuestionAnswers(string Answer, string answerId, int QuestionId)
        {
            try
            {
                // I. Check.
                // Get question.
                UserQuestionAnswersViewModel question = TestQuetions.Find(q => q.QuestionId == QuestionId);
                if (question == null)
                {
                    return RedirectToAction("Index");
                }
                if (!int.TryParse(answerId, out int intId))
                {
                    return RedirectToAction("Index");
                }

                // TO DO:
                //
                // Use method  Server.HtmlEncode() for encoding text in Answer field.
                // string encodeAnswer = Server.HtmlEncode(Answer);
                //

                // II. Get data for a view.
                UserAnswer userAnswer = new UserAnswer()
                {
                    Id = intId,
                    IsSelected = true,
                    Answer = Answer
                };
                UserAnswers[QuestionId].Clear();
                UserAnswers[QuestionId].Add(userAnswer);
                question.SelectedUserAnswers = UserAnswers[QuestionId];

                return PartialView(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Methods for CheckBox answers.
        [HttpGet]
        public ActionResult RadioButtonQuestionAnswers(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }

                // II. Get data for a view.
                // Get question.
                UserQuestionAnswersViewModel question = TestQuetions.Find(q => q.QuestionId == intId);
                if (question == null)
                {
                    return RedirectToAction("Index");
                }

                question.SelectedUserAnswers = UserAnswers[intId];

                return PartialView(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RadioButtonQuestionAnswers(int IsSelected, int QuestionId)
        {
            UserAnswer userAnswer;
            try
            {
                UserAnswers[QuestionId].Clear();
                userAnswer = new UserAnswer()
                {
                    Id = IsSelected,
                    IsSelected = true,
                    Answer = TestQuetions.Find(q => q.QuestionId == QuestionId).AvailableUserAnswers.Where(a => a.Id == IsSelected).FirstOrDefault().Answer
                };
                UserAnswers[QuestionId].Add(userAnswer);
                UserQuestionAnswersViewModel question = TestQuetions.Find(q => q.QuestionId == QuestionId);
                question.SelectedUserAnswers = UserAnswers[QuestionId];
                return PartialView(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        // Get test results.
        public async Task<ActionResult> GetCurrentTestResults(int? id)
        {
            try
            {
                // I. Clear controller properties for processing a test period.
                TestPeriod = 0;
                StartTestTime = null;

                // II. Check.
                if (AllAnswers.Count == 0 || UserAnswers.Count == 0)
                {
                    return RedirectToAction("Index");
                }

                // III.Get the test result.
                TestResultViewModel testResult = PLRepository.CalculateTestResults(AllAnswers, UserAnswers, TestQuetions);
                // Is the test passed?
                int firstQuestionId = AllAnswers.FirstOrDefault().Key;
                CourseDTO courseDTO = (await QuestionService.GetAsync(firstQuestionId)).Topic.Course;
                testResult.IsPassedTest = testResult.Result > testResult.MaxScore * courseDTO.PassingScore / 100;
                // Set ViewBag property for a View.
                ViewBag.CourseName = courseDTO.CourseTitle;

                // IV.Set testResult properies (Result, MaxScore and TestResultDetails).
                testResult.Result = testResult.Result * 1000 / testResult.MaxScore;
                testResult.MaxScore = 1000;
                foreach (var item in testResult.TestResultDetails)
                {
                    item.Topic = (await QuestionService.GetAsync(item.QuestionId)).Topic.TopicTitle;
                    item.Question = (await QuestionService.GetAsync(item.QuestionId)).QuestionText;
                }

                // V.
                return View(testResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // This method saves User test results if User unloads a test page unsaving results.
        [HttpPost]
        public async Task SetCurrentTestResults(int? id)
        {
            try
            {
                // I. Clear controller properties for processing a test period.
                //TestPeriod = 0;
                //StartTestTime = null;

                // II. Check.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null || AllAnswers.Count == 0 || UserAnswers.Count == 0)
                {
                    return;
                }

                // III.Get the test result.
                TestResultViewModel testResult = PLRepository.CalculateTestResults(AllAnswers, UserAnswers, TestQuetions);
                // Is the test passed?
                int firstQuestionId = AllAnswers.FirstOrDefault().Key;
                CourseDTO courseDTO = (await QuestionService.GetAsync(firstQuestionId)).Topic.Course;
                testResult.IsPassedTest = testResult.Result > testResult.MaxScore * courseDTO.PassingScore / 100;

                // IV. Write the test results to DB.
                OperationDetails operationDetails = await SaveCurrentTestResults(id, currentUserId, testResult);
                if (!operationDetails.Succedeed)
                {
                    throw new Exception (operationDetails.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        // Private method to save User test results.
        private async Task<OperationDetails> SaveCurrentTestResults(int? id, string currentUserId, TestResultViewModel testResult)
        {
            bool isTopicTest;
            OperationDetails operationDetails;
            try
            {
                // I. Checks.
                if (id != null)
                {
                    if (!int.TryParse(id.ToString(), out int intId) || intId > 1 || intId < 0)
                    {
                        operationDetails = new OperationDetails(false, "Incorrect parameter id!", "TestController.SaveCurrentTestResults");
                        return operationDetails;
                    }
                    if (intId == 1)
                        isTopicTest = true;
                    else
                        isTopicTest = false;
                }
                else
                {
                    operationDetails = new OperationDetails(false, "Incorrect parameter id!", "TestController.SaveCurrentTestResults");
                    return operationDetails;
                }

                // II. Create TestResultDTO object and set its properties.
                TestResultDTO testResultDTO = new TestResultDTO()
                {
                    UserProfileId = currentUserId,
                    Result = testResult.Result,
                    MaxScore = testResult.MaxScore,
                    IsPassedTest = testResult.IsPassedTest,
                    IsTopicTest = isTopicTest
                };

                // III. Add a test result to DB.
                operationDetails = await TestResultService.CreateAsync(testResultDTO, currentUserId);
                if (!operationDetails.Succedeed)
                {
                    return operationDetails;
                }

                // IV. Add test result details to DB.
                foreach (var item in testResult.TestResultDetails)
                {
                    TestResultDetailDTO testResultDetailDTO = new TestResultDetailDTO()
                    {
                        TestResultId = testResultDTO.TestResultId,
                        QuestionId = item.QuestionId,
                        IsProperAnswer = item.IsProperAnswer
                    };
                    operationDetails = await TestResultDetailService.CreateAsync(testResultDetailDTO, currentUserId);
                    if (!operationDetails.Succedeed)
                    {
                        return operationDetails;
                    }
                }

                // V.
                operationDetails = new OperationDetails(true, "Test results have successfully added to DB!", "TestController.SaveCurrentTestResults");
                return operationDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}