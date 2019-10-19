using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.WEB.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.Infrastructure;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;
using SiteWithAuthentication.WEB.Util;

namespace SiteWithAuthentication.WEB.Controllers
{
    [Authorize(Roles = "admin, moderator, user")]
    public class ModeratorSubscriptionController : Controller
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
        private ICommonService<SubscriptionForModeratorDTO> SubscriptionForModeratorService
        {
            get
            {
                return bLLUnitOfWork.SubscriptionForModeratorService;
            }
        }
        #endregion

        // GET: ModeratorSuscription/Index
        public ActionResult Index()
        {
            try
            {
                //I. Check.
                // Get Id for the current user.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // II. Set ViewBag properties.
                PLRepository.SetViewBagProperiesForModeratorSubscription(UserService, currentUserId,
                                                                         SubscriptionForModeratorService,
                                                                         out bool? viewBagIsAllowedToSuggest,
                                                                         out bool? viewBagGoToTrialSubscription,
                                                                         out bool? viewBagGoToSubscription);
                ViewBag.IsAllowedToSuggest = viewBagIsAllowedToSuggest;
                ViewBag.GoToTrialSubscription = viewBagGoToTrialSubscription;
                ViewBag.GoToSubscription = viewBagGoToSubscription;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SubjectDTO, SubjectViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.SubjectId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.SubjectName));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IEnumerable<SubjectViewModel> subjects = iMapper.Map<IEnumerable<SubjectDTO>, IEnumerable<SubjectViewModel>>(SubjectService.Find(subject => subject.IsApproved))
                    .OrderBy(obj => obj.Name);

                // V.
                return View(subjects);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Subsription methods
        // GET: ModeratorSuscription/Subscribe
        public ActionResult Subscribe()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Subscribe(ModeratorSubscriptionViewModel subscription)
        {
            try
            {
                //I. Check.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return new HttpUnauthorizedResult();
                }

                // II. Create a moderator subscription.
                if (ModelState.IsValid)
                {
                    SubscriptionForModeratorDTO subscriptionDTO = new SubscriptionForModeratorDTO
                    {
                        UserProfileId = currentUserId,
                        CourseCount = subscription.CourseCount,
                        SubscriptionPeriod = subscription.SubscriptionPeriod,
                        IsTrial = false,
                        IsApproved = false
                    };
                    OperationDetails operationDetails = await SubscriptionForModeratorService.CreateAsync(subscriptionDTO, currentUserId);
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

                // III.
                ViewBag.Message = "Non valid";
                return PartialView(subscription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ModeratorSuscription/TrialSubscribe
        public async Task<ActionResult> TrialSubscribe()
        {
            try
            {
                //I. Check.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return new HttpUnauthorizedResult();
                }

                // II. Create a trial moderator subscription.
                if (ModelState.IsValid)
                {
                    SubscriptionForModeratorDTO subscriptionDTO = new SubscriptionForModeratorDTO
                    {
                        UserProfileId = currentUserId,
                        CourseCount = 1,
                        SubscriptionPeriod = 7,
                        IsTrial = true,
                        IsApproved = true
                    };
                    OperationDetails operationDetails = await SubscriptionForModeratorService.CreateAsync(subscriptionDTO, currentUserId);
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

                // III.
                ViewBag.Message = "Non valid";
                return PartialView();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Subject methods
        // Suggest a subject.
        [Authorize(Roles = "admin, moderator")]
        public ActionResult SuggestSubject()
        {
            return PartialView();
        }
        [Authorize(Roles = "admin, moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuggestSubject(SubjectViewModel subject)
        {
            try
            {
                //I. Check.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return new HttpUnauthorizedResult();
                }

                // II. Create a new subject.
                if (ModelState.IsValid)
                {
                    SubjectDTO subjectDTO = new SubjectDTO
                    {
                        SubjectName = subject.Name,
                        Description = subject.Description,
                        IsApproved = false
                    };
                    OperationDetails operationDetails = await SubjectService.CreateAsync(subjectDTO, currentUserId);
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

                // III.
                ViewBag.Message = "Non valid";
                return PartialView(subject);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Speciality methods
        // Subject specialities list.
        public async Task<ActionResult> SubjectSpecialities(int? id)
        {
            try
            {
                // I. Checks.
                // Get Id for the current user.
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
                // Get SubjectDTO.
                SubjectDTO subjectDTO = await SubjectService.GetAsync(intId);
                if (subjectDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                PLRepository.SetViewBagProperiesForModeratorSubscription(UserService, currentUserId,
                                                                         SubscriptionForModeratorService,
                                                                         out bool? viewBagIsAllowedToSuggest,
                                                                         out bool? viewBagGoToTrialSubscription,
                                                                         out bool? viewBagGoToSubscription);
                ViewBag.IsAllowedToSuggest = viewBagIsAllowedToSuggest;
                ViewBag.GoToTrialSubscription = viewBagGoToTrialSubscription;
                ViewBag.GoToSubscription = viewBagGoToSubscription;
                ViewBag.ParentId = intId;
                ViewBag.SubjectName = subjectDTO.SubjectName;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SpecialityDTO, SpecialityViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.SpecialityId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.SpecialityName));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                List<SpecialityDTO> source = SpecialityService.Find(dto => dto.SubjectId == intId && dto.IsApproved).OrderBy(o => o.SpecialityName).ToList();
                IEnumerable<SpecialityViewModel> specialityOrderedList = iMapper.Map<IEnumerable<SpecialityDTO>, IEnumerable<SpecialityViewModel>>(source);

                // V.
                return View(specialityOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Suggest a speciality.
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> SuggestSpeciality(int? subjectId)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(subjectId.ToString(), out int intSubjectId))
                {
                    return RedirectToAction("Index");
                }
                // Get a SubjectDTO object.
                SubjectDTO subject = await SubjectService.GetAsync(intSubjectId);
                if (subject == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = intSubjectId;
                ViewBag.SubjectName = subject.SubjectName;

                // III.
                return PartialView();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> SuggestSpeciality(SpecialityViewModel speciality)
        {
            try
            {
                // I. Checks.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return new HttpUnauthorizedResult();
                }

                // II. Set ViewBag properties.
                ViewBag.SubjectName = (await SubjectService.GetAsync(speciality.SubjectId)).SubjectName;
                ViewBag.ParentId = speciality.SubjectId;
                ViewBag.Action = "SubjectSpecialities";

                // III. Create a new speciality.
                if (ModelState.IsValid)
                {
                    SpecialityDTO specialityDTO = new SpecialityDTO
                    {
                        SubjectId = speciality.SubjectId,
                        SpecialityName = speciality.Name,
                        Description = speciality.Description,
                        IsApproved = false
                    };
                    OperationDetails operationDetails = await SpecialityService.CreateAsync(specialityDTO, currentUserId);
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

                // IV.
                ViewBag.Message = "Non valid";
                return PartialView(speciality);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Course methods
        // Speciality courses list.
        public async Task<ActionResult> SpecialityCourses(int? id)
        {
            try
            {
                // I. Checks.
                // Get Id for the current user.
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
                // Get SubjectDTO.
                SpecialityDTO specialityDTO = await SpecialityService.GetAsync(intId);
                if (specialityDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = intId;
                ViewBag.SpecialityName = specialityDTO.SpecialityName;
                ViewBag.ParentParentId = specialityDTO.SubjectId;
                // If a current user is Admin.
                if (UserService.FindUserRoleById(currentUserId).ToLower() == "admin")
                {
                    ViewBag.IsAllowToAddCourses = true;
                    ViewBag.IsAdmin = true;
                }
                else
                {
                    ViewBag.IsAdmin = false;
                    PLRepository.SetViewBagProperiesForModeratorSubscription(UserService, currentUserId,
                                                                             SubscriptionForModeratorService,
                                                                             out bool? viewBagIsAllowedToSuggest,
                                                                             out bool? viewBagGoToTrialSubscription,
                                                                             out bool? viewBagGoToSubscription);
                    ViewBag.GoToTrialSubscription = viewBagGoToTrialSubscription;
                    ViewBag.GoToSubscription = viewBagGoToSubscription;
                    // Count of existed courses.
                    int existedCourseCount = CourseService.Find(dto => dto.UserProfileId == currentUserId).Count();
                    // Get count of allowed courses according to the active subscription.
                    IEnumerable<SubscriptionForModeratorDTO> subscriptions = SubscriptionForModeratorService.Find(obj => 
                                                                             obj.UserProfileId == currentUserId 
                                                                             && obj.IsApproved);
                    if (subscriptions.Count() == 0)
                    {
                        ViewBag.IsAllowToAddCourses = false;
                    }
                    else
                    {
                        int allowedCourseCount = (from sub in subscriptions
                                                  where sub.StartDate < DateTime.Now
                                                  && (DateTime.Now - sub.StartDate < TimeSpan.FromDays(sub.SubscriptionPeriod)
                                                  && sub.IsApproved)
                                                  select sub).FirstOrDefault().CourseCount;
                        bool isAllowToAddCourses = allowedCourseCount > existedCourseCount;
                        if (isAllowToAddCourses)
                        {
                            ViewBag.MessageAboutAllowedCourses = string.Format("According to terms of your subscription you are allowed {0} course(s). You can add {1} more course(s).",
                                                                               allowedCourseCount, allowedCourseCount - existedCourseCount);
                        }
                        else
                        {
                            ViewBag.MessageAboutAllowedCourses = "According to terms of your subscription you can't add a new course. Delete an unnecessary course or upgrade to a new subscription.";
                        }
                        ViewBag.IsAllowToAddCourses = isAllowToAddCourses;
                    }
                }

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IEnumerable<CourseDTO> source = CourseService.Find(dto => dto.SpecialityId == intId && dto.UserProfileId == currentUserId)
                    .OrderBy(o => o.CourseTitle)
                    .OrderBy(o => o.IsApproved);
                IEnumerable<CourseViewModel> courseOrderedList = iMapper.Map<IEnumerable<CourseDTO>, IEnumerable<CourseViewModel>>(source);

                // V.
                return View(courseOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Create a course.
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> CreateCourse(int? specialityId)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(specialityId.ToString(), out int intSpecialityId))
                {
                    return RedirectToAction("Index");
                }
                // Get a SpecialityDTO object.
                SpecialityDTO speciality = await SpecialityService.GetAsync(intSpecialityId);
                if (speciality == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = intSpecialityId;
                ViewBag.SpecialityName = speciality.SpecialityName;

                // III.
                return PartialView();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult> CreateCourse(CourseViewModel course)
        {
            try
            {
                // I. Checks.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return new HttpUnauthorizedResult();
                }

                // II. Set ViewBag properties.
                ViewBag.SpecialityName = (await SpecialityService.GetAsync(course.SpecialityId)).SpecialityName;
                ViewBag.ParentId = course.SpecialityId;
                ViewBag.Action = "SpecialityCourses";

                // III. Create a new course.
                if (ModelState.IsValid)
                {
                    CourseDTO courseDTO = new CourseDTO
                    {
                        UserProfileId = currentUserId,
                        SpecialityId = course.SpecialityId,
                        CourseTitle = course.Name,
                        Description = course.Description,
                        CourseTestQuestionsNumber = course.CourseTestQuestionsNumber,
                        TopicTestQuestionsNumber = course.TopicTestQuestionsNumber,
                        TimeToAnswerOneQuestion = course.TimeToAnswerOneQuestion,
                        AttemptsNumber = course.AttemptsNumber,
                        PassingScore = course.PassingScore,
                        IsApproved = course.IsApproved,
                        IsFree = course.IsFree
                    };
                    OperationDetails operationDetails = await CourseService.CreateAsync(courseDTO, currentUserId);
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

                // IV.
                ViewBag.Message = "Non valid";
                return PartialView(course);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}