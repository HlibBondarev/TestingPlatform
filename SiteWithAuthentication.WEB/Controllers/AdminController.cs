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

namespace SiteWithAuthentication.WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IBLLUnitOfWork bLLUnitOfWork = new BLLUnitOfWork("DefaultConnection");

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
        private ICommonService<CourseAssignmentDTO> CourseAssignmentService
        {
            get
            {
                return bLLUnitOfWork.CourseAssignmentService;
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
        private ICommonService<SubscriptionForModeratorDTO> SubscriptionForModeratorService
        {
            get
            {
                return bLLUnitOfWork.SubscriptionForModeratorService;
            }
        }
        #endregion

        // GET: Admin/Index
        public ActionResult Index()
        {
            // I. Set Session[ActiveController].
            Session["ActiveController"] = "Admin";

            // II. Get data for a view.
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubjectDTO, SubjectViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.SubjectId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.SubjectName));
            });
            IMapper iMapper = config.CreateMapper();
            IEnumerable<SubjectViewModel> subjects = iMapper.Map<IEnumerable<SubjectDTO>, IEnumerable<SubjectViewModel>>(SubjectService.GetAll())
                .OrderBy(obj => obj.Name)
                .OrderBy(obj => obj.IsApproved);
            return View(subjects);
        }

        #region Subject methods
        // Create a subject.
        public ActionResult CreateSubject()
        {
            return PartialView("~/Views/Admin/Subject/CreateSubject.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSubject(SubjectViewModel subject)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            if (ModelState.IsValid)
            {
                SubjectDTO subjectDTO = new SubjectDTO
                {
                    SubjectName = subject.Name,
                    Description = subject.Description,
                    IsApproved = subject.IsApproved
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
            ViewBag.Message = "Non valid";
            return PartialView("~/Views/Admin/Subject/CreateSubject.cshtml", subject);
        }

        // Edit a subject.
        public async Task<ActionResult> EditSubject(int? id)
        {
            // Check id.
            if (!int.TryParse(id.ToString(), out int intId))
            {
                return RedirectToAction("Index");
            }
            // Get SubjectDTO.
            SubjectDTO source = await SubjectService.GetAsync(intId);
            if (source == null)
            {
                return RedirectToAction("Index");
            }
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubjectDTO, SubjectViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.SubjectId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.SubjectName));
            });
            IMapper iMapper = config.CreateMapper();
            SubjectViewModel subject = iMapper.Map<SubjectDTO, SubjectViewModel>(source);
            return PartialView("~/Views/Admin/Subject/EditSubject.cshtml", subject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubject(SubjectViewModel subject)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            if (ModelState.IsValid)
            {
                SubjectDTO subjectDTO = new SubjectDTO
                {
                    SubjectId = subject.Id,
                    SubjectName = subject.Name,
                    Description = subject.Description,
                    IsApproved = subject.IsApproved
                };
                OperationDetails operationDetails = await SubjectService.UpdateAsync(subjectDTO, currentUserId);
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
            return PartialView("~/Views/Admin/Subject/EditSubject.cshtml", subject);
        }

        // Delete a subject.
        public async Task<ActionResult> DeleteSubject(int? id)
        {
            // Check id.
            if (!int.TryParse(id.ToString(), out int intId))
            {
                return RedirectToAction("Index");
            }
            // Get SubjectDTO.
            SubjectDTO source = await SubjectService.GetAsync(intId);
            if (source == null)
            {
                return RedirectToAction("Index");
            }
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubjectDTO, SubjectViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.SubjectId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.SubjectName));
            });
            IMapper iMapper = config.CreateMapper();
            SubjectViewModel subject = iMapper.Map<SubjectDTO, SubjectViewModel>(source);
            return PartialView("~/Views/Admin/Subject/DeleteSubject.cshtml", subject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            SubjectDTO subjectDTO = await SubjectService.GetAsync(id);
            if (subjectDTO != null)
            {
                OperationDetails operationDetails = await SubjectService.DeleteAsync(id, currentUserId);
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
            return PartialView("~/Views/Admin/Subject/DeleteSubject.cshtml", id);
        }
        #endregion

        #region Speciality methods
        // Subject specialities list.
        public async Task<ActionResult> SubjectSpecialities(int? id)
        {
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
            // Set ViewBag properties.
            ViewBag.ParentId = intId;
            ViewBag.SubjectName = subjectDTO.SubjectName;
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SpecialityDTO, SpecialityViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.SpecialityId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.SpecialityName));
            });
            IMapper iMapper = config.CreateMapper();
            List<SpecialityDTO> source = SpecialityService.Find(dto => dto.SubjectId == intId).OrderBy(o => o.SpecialityName).OrderBy(o => o.IsApproved).ToList();
            IEnumerable<SpecialityViewModel> specialityOrderedList = iMapper.Map<IEnumerable<SpecialityDTO>, IEnumerable<SpecialityViewModel>>(source);
            return View(specialityOrderedList);
        }

        // Create a speciality.
        public async Task<ActionResult> CreateSpeciality(int? subjectId)
        {
            // Check id.
            if (!int.TryParse(subjectId.ToString(), out int intSubjectId))
            {
                return RedirectToAction("Index");
            }
            // Get SubjectDTO.
            SubjectDTO subject = await SubjectService.GetAsync(intSubjectId);
            if (subject == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = intSubjectId;
            ViewBag.SubjectName = subject.SubjectName;
            return PartialView("~/Views/Admin/Speciality/CreateSpeciality.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSpeciality(SpecialityViewModel speciality)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            ViewBag.SubjectName = (await SubjectService.GetAsync(speciality.SubjectId)).SubjectName;
            ViewBag.ParentId = speciality.SubjectId;
            ViewBag.Action = "SubjectSpecialities";
            if (ModelState.IsValid)
            {
                SpecialityDTO specialityDTO = new SpecialityDTO
                {
                    SubjectId = speciality.SubjectId,
                    SpecialityName = speciality.Name,
                    Description = speciality.Description,
                    IsApproved = speciality.IsApproved
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
            ViewBag.Message = "Non valid";
            return PartialView("~/Views/Admin/Speciality/CreateSpeciality.cshtml", speciality);
        }

        // Edit a speciality.
        public async Task<ActionResult> EditSpeciality(int? id)
        {
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
            ViewBag.SubjectName = specialityDTO.Subject.SubjectName;
            ViewBag.ParentId = specialityDTO.SubjectId;
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SpecialityDTO, SpecialityViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.SpecialityId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.SpecialityName));
            });
            IMapper iMapper = config.CreateMapper();
            SpecialityViewModel speciality = iMapper.Map<SpecialityDTO, SpecialityViewModel>(specialityDTO);
            return PartialView("~/Views/Admin/Speciality/EditSpeciality.cshtml", speciality);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSpeciality(SpecialityViewModel speciality)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            ViewBag.SubjectName = (await SpecialityService.GetAsync(speciality.Id)).Subject.SubjectName;
            ViewBag.ParentId = speciality.SubjectId;
            ViewBag.Action = "SubjectSpecialities";
            if (ModelState.IsValid)
            {
                SpecialityDTO specialityDTO = new SpecialityDTO
                {
                    SpecialityId = speciality.Id,
                    SubjectId = speciality.SubjectId,
                    SpecialityName = speciality.Name,
                    Description = speciality.Description,
                    IsApproved = speciality.IsApproved
                };
                OperationDetails operationDetails = await SpecialityService.UpdateAsync(specialityDTO, currentUserId);
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
            return PartialView("~/Views/Admin/Speciality/EditSpeciality.cshtml", speciality);
        }

        // Delete a speciality.
        public async Task<ActionResult> DeleteSpeciality(int? id)
        {
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
            ViewBag.SubjectName = specialityDTO.Subject.SubjectName;
            ViewBag.ParentId = specialityDTO.SubjectId;
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SpecialityDTO, SpecialityViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.SpecialityId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.SpecialityName));
            });
            IMapper iMapper = config.CreateMapper();
            SpecialityViewModel speciality = iMapper.Map<SpecialityDTO, SpecialityViewModel>(specialityDTO);
            return PartialView("~/Views/Admin/Speciality/DeleteSpeciality.cshtml", speciality);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSpeciality(int id)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            SpecialityDTO specialityDTO = await SpecialityService.GetAsync(id);
            ViewBag.SubjectName = specialityDTO.Subject.SubjectName;
            ViewBag.ParentId = specialityDTO.SubjectId;
            ViewBag.Action = "SubjectSpecialities";
            if (specialityDTO != null)
            {
                OperationDetails operationDetails = await SpecialityService.DeleteAsync(id, currentUserId);
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
            return PartialView("~/Views/Admin/Speciality/DeleteSpeciality.cshtml", specialityDTO);
        }
        #endregion

        #region Course methods
        // Speciality courses list.
        public async Task<ActionResult> SpecialityCourses(int? id)
        {
            // I. Checks.
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

            // II. Set ViewBag properties and Session[ParentParentIdForAdminController].
            ViewBag.ParentId = intId;
            ViewBag.SpecialityName = specialityDTO.SpecialityName;
            ViewBag.ParentParentId = specialityDTO.SubjectId;
            // Set Session[ParentParentIdForAdminController].
            Session["ParentParentIdForAdminController"] = intId;

            // III. Get data for a view.
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourseDTO, CourseViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
            });
            IMapper iMapper = config.CreateMapper();
            List<CourseDTO> source = CourseService.Find(dto => dto.SpecialityId == intId)
                .OrderBy(o => o.CourseTitle)
                .OrderBy(o => o.IsApproved).ToList();
            IEnumerable<CourseViewModel> courseOrderedList = iMapper.Map<IEnumerable<CourseDTO>, IEnumerable<CourseViewModel>>(source);
            return View(courseOrderedList);
        }

        // Create a course.
        public async Task<ActionResult> CreateCourse(int? specialityId)
        {
            // Check id.
            if (!int.TryParse(specialityId.ToString(), out int intId))
            {
                return RedirectToAction("Index");
            }
            // Get SpecialityDTO object.
            SpecialityDTO speciality = await SpecialityService.GetAsync(intId);
            if (speciality == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = intId;
            ViewBag.SpecialityName = speciality.SpecialityName;
            return PartialView("~/Views/Admin/Course/CreateCourse.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCourse(CourseViewModel course)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            ViewBag.SpecialityName = (await SpecialityService.GetAsync(course.SpecialityId)).SpecialityName;
            ViewBag.ParentId = course.SpecialityId;
            ViewBag.Action = "SpecialityCourses";
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
            ViewBag.Message = "Non valid";
            return PartialView("~/Views/Admin/Course/CreateCourse.cshtml", course);
        }

        // Edit a course.
        public async Task<ActionResult> EditCourse(int? id)
        {
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
            ViewBag.ParentId = courseDTO.SpecialityId;
            ViewBag.SpecialityName = courseDTO.Speciality.SpecialityName;
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourseDTO, CourseViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
            });
            IMapper iMapper = config.CreateMapper();
            CourseViewModel course = iMapper.Map<CourseDTO, CourseViewModel>(courseDTO);
            return PartialView("~/Views/Admin/Course/EditCourse.cshtml", course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCourse(CourseViewModel course)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            ViewBag.SpecialityName = (await CourseService.GetAsync(course.Id)).Speciality.SpecialityName;
            ViewBag.ParentId = course.SpecialityId;
            ViewBag.Action = "SpecialityCourses";
            if (ModelState.IsValid)
            {
                CourseDTO courseDTO = new CourseDTO
                {
                    CourseId = course.Id,
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
                OperationDetails operationDetails = await CourseService.UpdateAsync(courseDTO, currentUserId);
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
            return PartialView("~/Views/Admin/Course/EditCourse.cshtml", course);
        }

        // Delete a course.
        public async Task<ActionResult> DeleteCourse(int? id)
        {
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
            ViewBag.SpecialityName = courseDTO.Speciality.SpecialityName;
            ViewBag.ParentId = courseDTO.SpecialityId;
            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourseDTO, CourseViewModel>()
                .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
            });
            IMapper iMapper = config.CreateMapper();
            CourseViewModel source = iMapper.Map<CourseDTO, CourseViewModel>(courseDTO);
            return PartialView("~/Views/Admin/Course/DeleteCourse.cshtml", source);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return new HttpUnauthorizedResult();
            }
            CourseDTO courseDTO = await CourseService.GetAsync(id);
            ViewBag.SpecialityName = courseDTO.Speciality.SpecialityName;
            ViewBag.ParentId = courseDTO.SpecialityId;
            ViewBag.Action = "SpecialityCourses";
            if (courseDTO != null)
            {
                OperationDetails operationDetails = await CourseService.DeleteAsync(id, currentUserId);
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
            return PartialView("~/Views/Admin/Course/DeleteCourse.cshtml", courseDTO);
        }
        #endregion

        #region ModeratorSubcription methods
        public ActionResult ModeratorSubcriptions()
        {          
            return View();
        }

        public JsonResult GetModeratorSubcriptions(string sidx, string sort, int page, int rows,
            bool _search, string searchField, string searchOper, string searchString)
        {
            sort = sort ?? "";
            int pageIndex = page - 1;
            int pageSize = rows;

            // AutoMapper Setup.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubscriptionForModeratorDTO, ModeratorSubscriptionViewModel>()
                .ForMember("ID", opt => opt.MapFrom(obj => obj.SubscriptionForModeratorId))
                .ForMember("StartDate", opt => opt.MapFrom(obj => obj.StartDate.ToShortDateString()));
            });
            IMapper iMapper = config.CreateMapper();
            IEnumerable<SubscriptionForModeratorDTO> source = SubscriptionForModeratorService.GetAll();
            IEnumerable<ModeratorSubscriptionViewModel> subscriptionList = iMapper.Map<IEnumerable<SubscriptionForModeratorDTO>, IEnumerable<ModeratorSubscriptionViewModel>>(source).ToList();
            // Search.
            if (_search)
            {
                switch (searchField)
                {
                    case "Email":
                        switch (searchOper)
                        {
                            case "cn":
                                subscriptionList = subscriptionList.Where(t => t.Email.Contains(searchString));
                                break;
                            case "eq":
                                subscriptionList = subscriptionList.Where(t => t.Email == searchString);
                                break;
                        }
                        break;
                    case "StartDate":
                        switch (searchOper)
                        {
                            case "cn":
                                subscriptionList = subscriptionList.Where(t => t.StartDate.Contains(searchString));
                                break;
                            case "eq":
                                subscriptionList = subscriptionList.Where(t => t.StartDate == searchString);
                                break;
                        }
                        break;
                    case "IsTrial":
                        switch (searchOper)
                        {
                            case "cn":
                                if (bool.TryParse(searchString, out bool isCnTrial))
                                {
                                    subscriptionList = subscriptionList.Where(t => t.IsTrial == isCnTrial);
                                }
                                break;
                            case "eq":
                                if (bool.TryParse(searchString, out bool isEqTrial))
                                {
                                    subscriptionList = subscriptionList.Where(t => t.IsTrial == isEqTrial);
                                }
                                break;
                        }
                        break;
                    case "IsApproved":
                        switch (searchOper)
                        {
                            case "cn":
                                break;
                            case "eq":
                                if (bool.TryParse(searchString, out bool isApproved))
                                {
                                    subscriptionList = subscriptionList.Where(t => t.IsApproved == isApproved);
                                }
                                break;
                        }
                        break;
                }
            }
            // Sort.
            int totalRecords = subscriptionList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                subscriptionList = subscriptionList.OrderByDescending(t => t.Email);
                subscriptionList = subscriptionList.OrderByDescending(t => t.IsApproved);
                subscriptionList = subscriptionList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                subscriptionList = subscriptionList.OrderBy(t => t.Email);
                subscriptionList = subscriptionList.OrderBy(t => t.IsApproved);
                subscriptionList = subscriptionList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = subscriptionList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<string> EditModeratorSubscription(ModeratorSubscriptionViewModel Model)
        {
            string msg;
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return "User isn't authorized!";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    SubscriptionForModeratorDTO subscriptionDTO = new SubscriptionForModeratorDTO
                    {
                        SubscriptionForModeratorId = Model.ID,
                        SubscriptionPeriod = Model.SubscriptionPeriod,
                        CourseCount = Model.CourseCount,
                        IsApproved = Model.IsApproved
                    };
                    OperationDetails operationDetails = await SubscriptionForModeratorService.UpdateAsync(subscriptionDTO, currentUserId);
                    if (operationDetails.Succedeed)
                    {
                        msg = "Saved Successfully";
                    }
                    else
                    {
                        ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
                        msg = string.Format("Validation data not successfully. Error in {0}. Error message: {1}", operationDetails.Property, operationDetails.Message);
                    }
                }
                else
                {
                    msg = "Validation data not successfully";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        public async Task<string> DeleteModeratorSubscription(int id)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return "User isn't authorized!";
            }
            OperationDetails operationDetails = await SubscriptionForModeratorService.DeleteAsync(id, currentUserId);
            if (operationDetails.Succedeed)
            {
                return "Deleted successfully";
            }
            else
            {
                ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
                return string.Format("Deleting failed. Error in {0}. Error message: {1}", operationDetails.Property, operationDetails.Message);
            }
        }
        #endregion
    }
}
