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
using System.IO;
using System.Text;
using SiteWithAuthentication.WEB.Util;

namespace SiteWithAuthentication.WEB.Controllers
{
    [Authorize(Roles = "admin, moderator")]
    public class TestManagementController : Controller
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
        private ICommonService<SubscriptionDTO> SubscriptionService
        {
            get
            {
                return bLLUnitOfWork.SubscriptionService;
            }
        }
        #endregion

        // GET: TestManagement/Index
        public ActionResult Index()
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

                // II. Set Session[ActiveController].
                Session["ActiveController"] = "TestManagement";

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IEnumerable<CourseDTO> source = CourseService.Find(dto => dto.UserProfileId == currentUserId)
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

        #region Course methods
        // Edit a course.
        public async Task<ActionResult> EditCourse(int? id)
        {
            try
            {
                // I. Checks.
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
                ViewBag.SpecialityName = courseDTO.Speciality.SpecialityName;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
                });
                IMapper iMapper = config.CreateMapper();
                // IV. Get data for a view.
                CourseViewModel course = iMapper.Map<CourseDTO, CourseViewModel>(courseDTO);

                // V.
                return PartialView(course);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCourse(CourseViewModel course)
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
                ViewBag.SpecialityName = (await CourseService.GetAsync(course.Id)).Speciality.SpecialityName;
                ViewBag.ParentId = course.SpecialityId;

                // III. Edit the selected course.
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

                // IV.
                ViewBag.Message = "Non valid";
                return PartialView(course);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Delete a course.
        public async Task<ActionResult> DeleteCourse(int? id)
        {
            try
            {
                // I. Checks.
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
                ViewBag.SpecialityName = courseDTO.Speciality.SpecialityName;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                CourseViewModel course = iMapper.Map<CourseDTO, CourseViewModel>(courseDTO);

                // V.
                return PartialView(course);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCourse(int id)
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
                CourseDTO courseDTO = await CourseService.GetAsync(id);
                ViewBag.SpecialityName = courseDTO.Speciality.SpecialityName;

                // III. Delete the selected course.
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

                // IV.
                ViewBag.Message = "Non valid";
                return PartialView(courseDTO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region CourseAssignment methods
        // Course assignments list.
        public async Task<ActionResult> CourseAssignments(int? id)
        {
            try
            {
                // I. Checks.
                //  Get Id for the current user.
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
                ViewBag.ParentId = intId;
                ViewBag.CourseName = courseDTO.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseAssignmentDTO, CourseAssignmentViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseAssignmentId));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                List<CourseAssignmentDTO> source = CourseAssignmentService.Find(dto => dto.CourseId == intId).OrderBy(o => o.Email).ToList();
                IEnumerable<CourseAssignmentViewModel> assignmentOrderedList = iMapper.Map<IEnumerable<CourseAssignmentDTO>, IEnumerable<CourseAssignmentViewModel>>(source);

                // V.
                return View(assignmentOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Create an assignment.
        public async Task<ActionResult> CreateAssignment(int? courseId)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(courseId.ToString(), out int intId))
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
                ViewBag.ParentId = intId;
                ViewBag.CourseName = courseDTO.CourseTitle;

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
        public async Task<ActionResult> CreateAssignment(CourseAssignmentViewModel assignment)
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
                ViewBag.CourseName = (await CourseService.GetAsync(assignment.CourseId)).CourseTitle;
                ViewBag.ParentId = assignment.CourseId;
                ViewBag.Action = "CourseAssignments";

                // III. Create a new assignment.
                if (ModelState.IsValid)
                {
                    CourseAssignmentDTO assignmentDTO = new CourseAssignmentDTO
                    {
                        Email = assignment.Email,
                        CourseId = assignment.CourseId,
                        IsApproved = assignment.IsApproved,
                    };
                    OperationDetails operationDetails = await CourseAssignmentService.CreateAsync(assignmentDTO, currentUserId);
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
                return PartialView(assignment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Searching method.
        public ActionResult AutoSearchEmail(string term)
        {
            try
            {
                var users = UserService.GetAll();
                var emails = users.Where(user => user.Email.Contains(term))
                                  .Select(user => new { value = user.Email })
                                  .Distinct();
                return Json(emails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Edit an assignment.
        public async Task<ActionResult> EditAssignment(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get CourseAssignmentDTO object.
                CourseAssignmentDTO courseAssignmentDTO = await CourseAssignmentService.GetAsync(intId);
                if (courseAssignmentDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = courseAssignmentDTO.CourseId;
                ViewBag.CourseName = courseAssignmentDTO.Course.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseAssignmentDTO, CourseAssignmentViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseAssignmentId));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                CourseAssignmentViewModel assignment = iMapper.Map<CourseAssignmentDTO, CourseAssignmentViewModel>(courseAssignmentDTO);
                ViewBag.Email = assignment.Email;

                // V.
                return PartialView(assignment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAssignment(CourseAssignmentViewModel assignment)
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
                ViewBag.CourseName = (await CourseAssignmentService.GetAsync(assignment.Id)).Course.CourseTitle;
                ViewBag.ParentId = assignment.CourseId;
                ViewBag.Action = "CourseAssignments";

                // III. Edit the selected assignment.
                if (ModelState.IsValid)
                {
                    CourseAssignmentDTO assignmentDTO = new CourseAssignmentDTO
                    {
                        CourseAssignmentId = assignment.Id,
                        CourseId = assignment.CourseId,
                        Email = assignment.Email,
                        IsApproved = assignment.IsApproved,
                    };
                    OperationDetails operationDetails = await CourseAssignmentService.UpdateAsync(assignmentDTO, currentUserId);
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
                return PartialView(assignment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Delete an assignment.
        public async Task<ActionResult> DeleteAssignment(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get CourseAssignmentDTO object.
                CourseAssignmentDTO courseAssignmentDTO = await CourseAssignmentService.GetAsync(intId);
                if (courseAssignmentDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.CourseName = courseAssignmentDTO.Course.CourseTitle;
                ViewBag.ParentId = courseAssignmentDTO.CourseId;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseAssignmentDTO, CourseAssignmentViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseAssignmentId));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                CourseAssignmentViewModel assignment = iMapper.Map<CourseAssignmentDTO, CourseAssignmentViewModel>(courseAssignmentDTO);

                // V.
                return PartialView(assignment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAssignment(int id)
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
                CourseAssignmentDTO courseAssignmentDTO = await CourseAssignmentService.GetAsync(id);
                ViewBag.CourseName = courseAssignmentDTO.Course.CourseTitle;
                ViewBag.ParentId = courseAssignmentDTO.CourseId;
                ViewBag.Action = "CourseAssignments";

                // III. Delete the selected assignment.
                if (courseAssignmentDTO != null)
                {
                    OperationDetails operationDetails = await CourseAssignmentService.DeleteAsync(id, currentUserId);
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
                return PartialView(courseAssignmentDTO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Subscriptions methods
        // Course subscriptions list.
        public async Task<ActionResult> CourseSubscriptions(int? id)
        {
            try
            {
                // I. Checks.
                //  Get Id for the current user.
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
                ViewBag.ParentId = intId;
                ViewBag.CourseName = courseDTO.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SubscriptionDTO, SubscriptionViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.SubscriptionId));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IEnumerable<SubscriptionDTO> source = SubscriptionService.Find(dto => dto.CourseId == intId).OrderBy(o => o.Email);
                IEnumerable<SubscriptionViewModel> subscriptionOrderedList = iMapper.Map<IEnumerable<SubscriptionDTO>, IEnumerable<SubscriptionViewModel>>(source);

                // V.
                return View(subscriptionOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Create a subscription.
        public async Task<ActionResult> CreateSubscription(int? courseId)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(courseId.ToString(), out int intId))
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
                ViewBag.ParentId = intId;
                ViewBag.CourseName = courseDTO.CourseTitle;

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
        public async Task<ActionResult> CreateSubscription(SubscriptionViewModel subscription)
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
                ViewBag.CourseName = (await CourseService.GetAsync(subscription.CourseId)).CourseTitle;
                ViewBag.ParentId = subscription.CourseId;
                ViewBag.Action = "CourseSubscriptions";

                // III. Create a subscription.
                if (ModelState.IsValid)
                {
                    SubscriptionDTO subscriptionDTO = new SubscriptionDTO
                    {
                        Email = subscription.Email,
                        CourseId = subscription.CourseId,
                        SubscriptionPeriod = subscription.SubscriptionPeriod,
                        IsApproved = subscription.IsApproved
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

                // IV.
                ViewBag.Message = "Non valid";
                return PartialView(subscription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Edit a subscription.
        public async Task<ActionResult> EditSubscription(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get subscriptionDTO object.
                SubscriptionDTO subscriptionDTO = await SubscriptionService.GetAsync(intId);
                if (subscriptionDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = subscriptionDTO.CourseId;
                ViewBag.CourseName = subscriptionDTO.Course.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SubscriptionDTO, SubscriptionViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.SubscriptionId));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                SubscriptionViewModel subscription = iMapper.Map<SubscriptionDTO, SubscriptionViewModel>(subscriptionDTO);

                // V.
                ViewBag.Email = subscription.Email;
                return PartialView(subscription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubscription(SubscriptionViewModel subscription)
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
                ViewBag.CourseName = (await SubscriptionService.GetAsync(subscription.ID)).Course.CourseTitle;
                ViewBag.ParentId = subscription.CourseId;
                ViewBag.Action = "CourseSubscriptions";

                // III. Edit the selected subscription.
                if (ModelState.IsValid)
                {
                    SubscriptionDTO subscriptionDTO = new SubscriptionDTO
                    {
                        SubscriptionId = subscription.ID,
                        CourseId = subscription.CourseId,
                        Email = subscription.Email,
                        SubscriptionPeriod = subscription.SubscriptionPeriod,
                        IsApproved = subscription.IsApproved
                    };
                    OperationDetails operationDetails = await SubscriptionService.UpdateAsync(subscriptionDTO, currentUserId);
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
                return PartialView(subscription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Delete a subscription.
        public async Task<ActionResult> DeleteSubscription(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get SubscriptionDTO object.
                SubscriptionDTO subscriptionDTO = await SubscriptionService.GetAsync(intId);
                if (subscriptionDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.CourseName = subscriptionDTO.Course.CourseTitle;
                ViewBag.ParentId = subscriptionDTO.CourseId;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SubscriptionDTO, SubscriptionViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.SubscriptionId));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                SubscriptionViewModel subscription = iMapper.Map<SubscriptionDTO, SubscriptionViewModel>(subscriptionDTO);

                // V.
                return PartialView(subscription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSubscription(int id)
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
                SubscriptionDTO subscriptionDTO = await SubscriptionService.GetAsync(id);
                ViewBag.CourseName = subscriptionDTO.Course.CourseTitle;
                ViewBag.ParentId = subscriptionDTO.CourseId;
                ViewBag.Action = "CourseSubscriptions";

                // III. Delete the selected subscription.
                if (subscriptionDTO != null)
                {
                    OperationDetails operationDetails = await SubscriptionService.DeleteAsync(id, currentUserId);
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
                return PartialView(subscriptionDTO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Topic methods
        // Course topics list.
        public async Task<ActionResult> CourseTopics(int? id)
        {
            try
            {
                // I. Checks.
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
                ViewBag.ParentId = intId;
                ViewBag.CourseName = courseDTO.CourseTitle;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TopicDTO, TopicViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.TopicId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                List<TopicDTO> source = TopicService.Find(dto => dto.CourseId == intId).OrderBy(o => o.TopicNumber).ToList();
                IEnumerable<TopicViewModel> topicOrderedList = iMapper.Map<IEnumerable<TopicDTO>, IEnumerable<TopicViewModel>>(source);

                // V.
                return View(topicOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ActionResult> CourseTopicsInJqGrid(int? id)
        {
            try
            {
                // I. Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }

                // II. Set Sesion[] and ViewBag properties. 
                Session["CourseId"] = intId;
                CourseDTO course = await CourseService.GetAsync(intId);
                ViewBag.CourseName = course.CourseTitle;
                ViewBag.ParentId = intId;

                // III.
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JsonResult GetTopics(string sidx, string sort, int page, int rows,
            bool _search, string searchField, string searchOper, string searchString)
        {
            try
            {
                // I. Set vars values.
                sort = sort ?? "";
                int pageIndex = page - 1;
                int pageSize = rows;
                if (!int.TryParse(Session["CourseId"].ToString(), out int courseId))
                {
                    courseId = -1;
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TopicDTO, TopicViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.TopicId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.TopicTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Get the topic list. 
                IEnumerable<TopicDTO> source = TopicService.Find(dto => dto.CourseId == courseId);
                IEnumerable<TopicViewModel> topicList = iMapper.Map<IEnumerable<TopicDTO>, IEnumerable<TopicViewModel>>(source).ToList();

                // IV. Search.
                if (_search)
                {
                    switch (searchField)
                    {
                        case "Name":
                            switch (searchOper)
                            {
                                case "cn":
                                    topicList = topicList.Where(t => t.Name.Contains(searchString));
                                    break;
                                case "eq":
                                    topicList = topicList.Where(t => t.Name == searchString);
                                    break;
                            }
                            break;
                        case "TopicNumber":
                            switch (searchOper)
                            {
                                case "cn":
                                    topicList = topicList.Where(t => t.TopicNumber.ToString().Contains(searchString));
                                    break;
                                case "eq":
                                    topicList = topicList.Where(t => t.TopicNumber.ToString() == searchString);
                                    break;
                            }
                            break;
                        case "IsFree":
                            switch (searchOper)
                            {

                                case "cn":
                                    if (bool.TryParse(searchString, out bool isCnTrial))
                                    {
                                        topicList = topicList.Where(t => t.IsFree == isCnTrial);
                                    }
                                    break;
                                case "eq":
                                    if (bool.TryParse(searchString, out bool isEqTrial))
                                    {
                                        topicList = topicList.Where(t => t.IsFree == isEqTrial);
                                    }
                                    break;
                            }
                            break;
                    }
                }

                // V. Sort.
                int totalRecords = topicList.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                if (sort.ToUpper() == "DESC")
                {
                    topicList = topicList.OrderByDescending(t => t.TopicNumber);
                    topicList = topicList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                else
                {
                    topicList = topicList.OrderBy(t => t.TopicNumber);
                    topicList = topicList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = topicList
                };

                // VI.
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<string> CreateTopicInJqGrid([Bind(Exclude = "ID,CourseId")] TopicViewModel Model)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }

                if (!int.TryParse(Session["CourseId"].ToString(), out int courseId))
                {
                    courseId = -1;
                }

                Model.CourseId = courseId;
                if (ModelState.IsValid)
                {
                    TopicDTO topicDTO = new TopicDTO
                    {
                        CourseId = Model.CourseId,
                        TopicTitle = Model.Name,
                        Description = Model.Description,
                        TopicNumber = Model.TopicNumber,
                        IsFree = Model.IsFree
                    };
                    OperationDetails operationDetails = await TopicService.CreateAsync(topicDTO, currentUserId);
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
        public async Task<string> EditTopicInJqGrid(TopicViewModel Model)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }
                if (ModelState.IsValid)
                {
                    TopicDTO topicDTO = new TopicDTO
                    {
                        TopicId = Model.ID,
                        CourseId = Model.CourseId,
                        TopicTitle = Model.Name,
                        Description = Model.Description,
                        TopicNumber = Model.TopicNumber,
                        IsFree = Model.IsFree
                    };
                    OperationDetails operationDetails = await TopicService.UpdateAsync(topicDTO, currentUserId);
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
        public async Task<string> DeleteTopicInJqGrid(int id)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }
                OperationDetails operationDetails = await TopicService.DeleteAsync(id, currentUserId);
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
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        #endregion

        #region Question methods
        // Topic questions list.
        public async Task<ActionResult> TopicQuestions(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get TopicDTO object.
                TopicDTO topicDTO = await TopicService.GetAsync(intId);
                if (topicDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set ViewBag properties.
                ViewBag.ParentId = intId;
                ViewBag.TopicName = topicDTO.TopicTitle;
                ViewBag.ParentParentId = topicDTO.CourseId;

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<QuestionDTO, QuestionViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.QuestionId))
                    .ForMember("Question", opt => opt.MapFrom(obj => obj.QuestionText))
                    .ForMember("AnswerType", opt => opt.MapFrom(obj => obj.AnswerType.AnswerTypeDescription));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                List<QuestionDTO> source = QuestionService.Find(dto => dto.TopicId == intId).ToList();
                IEnumerable<QuestionViewModel> questionList = iMapper.Map<IEnumerable<QuestionDTO>, IEnumerable<QuestionViewModel>>(source);

                // V.
                return View(questionList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            const string userResourcesFolderPath = "Content/UserResources/";
            try
            {
                // I. Checks.
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return Json("Exception: User isn't authorized.\nFile hasn't been uploaded!");
                }
                if (Request.Files.Count > 1)
                {
                    return Json("Exception: You can select only one file.\nFile hasn't been uploaded!");
                }
                string strQuestionId = Request.Form["questionId"];
                if (!int.TryParse(strQuestionId, out int questionId))
                {
                    return Json("Exception: QuestionId doesn't have integer type.\nFile hasn't been uploaded!");
                }

                // II. Get all paths and names.
                string newFileName = null;
                string fullPathToNewFile = null;
                // Get the storage folder path.
                int topicId = (await QuestionService.GetAsync(questionId)).TopicId;
                string courseTitle = (await TopicService.GetAsync(topicId)).Course.CourseTitle;
                string folderPath = new StringBuilder(userResourcesFolderPath).Append(courseTitle).ToString();
                // Get the selected question from DB.
                QuestionDTO questionDTO = await QuestionService.GetAsync(questionId);
                // Get the uploaded file.
                HttpPostedFileBase upload;
                if (Request.Files.Count == 0)
                {
                    upload = null;
                }
                else
                {
                    upload = Request.Files[0];
                    // Create a new file name.
                    if (Session["FileNumber"] == null)
                    {
                        Session["FileNumber"] = 0;
                    }
                    // Get Session ["FileNumber"] from the memory, cast to int type, and add +1.
                    int number = (int)Session["FileNumber"] + 1;
                    // Save in memory Session ["FileNumber"] a new value - number.
                    Session["FileNumber"] = number;
                    newFileName = new StringBuilder(PLRepository.CreateNewFileName(number)).Append(Path.GetExtension(upload.FileName)).ToString();
                    // Build the full path to the new file.
                    fullPathToNewFile = new StringBuilder("~/").Append(folderPath).Append("/").Append(newFileName).ToString();
                }
                // III. Unsafe code.
                if (upload != null)
                {
                    // Check the file extension and the file size.
                    if (!PLRepository.IsValidImage(upload.FileName, upload.ContentLength))
                        return Json("File hasn't been uploaded!\nOnly files with the extension *.gif, *.png и *.jpg.are accepted for download. File size can be no more than 1 Mb.");
                }

                // Delete the file which existed before.
                if (!string.IsNullOrEmpty(questionDTO.ResourceRef))
                {
                    // Delete the resource file.
                    System.IO.File.Delete(Server.MapPath(new StringBuilder("~/").Append(questionDTO.ResourceRef).ToString()));
                    // Clear property - ResourceRef.
                    questionDTO.ResourceRef = null;
                }

                if (upload != null)
                {
                    // If the directory doesn't exist create it.
                    Directory.CreateDirectory(Server.MapPath(new StringBuilder("~/").Append(folderPath).ToString()));
                    // Save a file in the appropriated folder.
                    upload.SaveAs(Server.MapPath(fullPathToNewFile));
                    // Set property - ResourceRef.
                    questionDTO.ResourceRef = new StringBuilder(folderPath).Append("/").Append(newFileName).ToString();
                }

                // Update the selected question in DB.
                OperationDetails operationDetails = await QuestionService.UpdateAsync(questionDTO, currentUserId);
                if (operationDetails.Succedeed)
                {
                    if (upload != null)
                        return Json("File has been uploaded!");
                    else
                        return Json("File has been deleted!");
                }
                else
                {
                    throw new Exception(operationDetails.Message);
                }
            }
            catch (Exception ex)
            {
                return Json(string.Format("Exeption: {0}\nFile hasn't been uploaded! ", ex.Message));
            }
        }

        public async Task<ActionResult> TopicQuestionsInJqGrid(int? id)
        {
            try
            {

                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get TopicDTO object.
                TopicDTO topicDTO = await TopicService.GetAsync(intId);
                if (topicDTO == null)
                {
                    return RedirectToAction("Index");
                }

                // II. Set Sesion[] and ViewBag properties.
                Session["TopicId"] = intId;
                ViewBag.TopicName = topicDTO.TopicTitle;
                ViewBag.ParentId = intId;

                // III.
                return View();
            }
            catch (Exception ex)
            {
                return Json(string.Format("Exeption: {0}\nFile hasn't been uploaded! ", ex.Message));
            }
        }

        public JsonResult GetQuestions(string sidx, string sort, int page, int rows,
            bool _search, string searchField, string searchOper, string searchString)
        {
            try
            {
                // I. Set vars values.
                sort = sort ?? "";
                int pageIndex = page - 1;
                int pageSize = rows;
                if (!int.TryParse(Session["TopicId"].ToString(), out int topicId))
                {
                    topicId = -1;
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<QuestionDTO, QuestionViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.QuestionId))
                    .ForMember("Question", opt => opt.MapFrom(obj => obj.QuestionText))
                    .ForMember("AnswerType", opt => opt.MapFrom(obj => obj.AnswerType.AnswerTypeDescription));
                });
                IMapper iMapper = config.CreateMapper();
                IEnumerable<QuestionDTO> source = QuestionService.Find(dto => dto.TopicId == topicId);
                IEnumerable<QuestionViewModel> questionList = iMapper.Map<IEnumerable<QuestionDTO>, IEnumerable<QuestionViewModel>>(source).ToList();

                // III. Search.
                if (_search)
                {
                    switch (searchField)
                    {
                        case "Question":
                            switch (searchOper)
                            {
                                case "cn":
                                    questionList = questionList.Where(t => t.Question.Contains(searchString));
                                    break;
                                case "eq":
                                    questionList = questionList.Where(t => t.Question == searchString);
                                    break;
                            }
                            break;
                        case "AnswerType":
                            switch (searchOper)
                            {
                                case "cn":
                                    questionList = questionList.Where(t => t.AnswerType.Contains(searchString));
                                    break;
                                case "eq":
                                    questionList = questionList.Where(t => t.AnswerType == searchString);
                                    break;
                            }
                            break;
                    }
                }

                // IV. Sort.
                int totalRecords = questionList.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                if (sort.ToUpper() == "DESC")
                {
                    questionList = questionList.OrderByDescending(t => t.Question);
                    questionList = questionList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                else if (sort.ToUpper() == "ASC")
                {
                    questionList = questionList.OrderBy(t => t.Question);
                    questionList = questionList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = questionList
                };

                // V.
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(string.Format("Exeption: {0}\nFile hasn't been uploaded! ", ex.Message));
            }
        }

        [HttpPost]
        public async Task<string> CreateQuestionInJqGrid([Bind(Exclude = "ID,TopicId")] QuestionViewModel Model)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }

                if (!int.TryParse(Session["TopicId"].ToString(), out int topicId))
                {
                    topicId = -1;
                }

                Model.TopicId = topicId;
                if (ModelState.IsValid)
                {
                    QuestionDTO questionDTO = new QuestionDTO
                    {
                        TopicId = Model.TopicId,
                        AnswerTypeId = Model.AnswerTypeId,
                        QuestionText = Model.Question,
                        ResourceRef = Model.ResourceRef,
                        QuestionWeight = Model.QuestionWeight,
                    };
                    OperationDetails operationDetails = await QuestionService.CreateAsync(questionDTO, currentUserId);
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
        public async Task<string> EditQuestionInJqGrid(QuestionViewModel Model)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }
                if (ModelState.IsValid)
                {
                    QuestionDTO questionDTO = new QuestionDTO
                    {
                        QuestionId = Model.ID,
                        TopicId = Model.TopicId,
                        QuestionText = Model.Question,
                        AnswerTypeId = Model.AnswerTypeId,
                        ResourceRef = Model.ResourceRef,
                        QuestionWeight = Model.QuestionWeight
                    };
                    OperationDetails operationDetails = await QuestionService.UpdateAsync(questionDTO, currentUserId);
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
        public async Task<string> DeleteQuestionInJqGrid(int id)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return "User isn't authorized!";
            }
            OperationDetails operationDetails = await QuestionService.DeleteAsync(id, currentUserId);
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

        #region QuestionAnswersInJqGrid methods
        public async Task<ActionResult> QuestionAnswersInJqGrid(int? id)
        {
            try
            {
                // I. Checks.
                // Check id.
                if (!int.TryParse(id.ToString(), out int intId))
                {
                    return RedirectToAction("Index");
                }
                // Get TopicDTO object.
                QuestionDTO questionDTO = await QuestionService.GetAsync(intId);
                if (questionDTO == null)
                {
                    return RedirectToAction("Index");
                }
                // II. Set ViewBag properties.
                Session["QuestionId"] = intId;
                ViewBag.QuestionText = questionDTO.QuestionText;
                ViewBag.ParentParentId = questionDTO.TopicId;

                // III.
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JsonResult GetAnswers(string sidx, string sort, int page, int rows,
            bool _search, string searchField, string searchOper, string searchString)
        {
            try
            {
                // I. Set vars values.
                sort = sort ?? "";
                int pageIndex = page - 1;
                int pageSize = rows;
                if (!int.TryParse(Session["QuestionId"].ToString(), out int questionId))
                {
                    questionId = -1;
                }

                // II. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<AnswerDTO, AnswerViewModel>()
                    .ForMember("ID", opt => opt.MapFrom(obj => obj.AnswerId))
                    .ForMember("Answer", opt => opt.MapFrom(obj => obj.AnswerText));
                });
                IMapper iMapper = config.CreateMapper();

                // III. Get answers list.
                IEnumerable<AnswerDTO> source = AnswerService.Find(dto => dto.QuestionId == questionId).ToList();
                IEnumerable<AnswerViewModel> answerList = iMapper.Map<IEnumerable<AnswerDTO>, IEnumerable<AnswerViewModel>>(source).ToList();

                // IV. Search.
                if (_search)
                {
                    switch (searchField)
                    {
                        case "Answer":
                            switch (searchOper)
                            {
                                case "cn":
                                    answerList = answerList.Where(t => t.Answer.Contains(searchString));
                                    break;
                                case "eq":
                                    answerList = answerList.Where(t => t.Answer == searchString);
                                    break;
                            }
                            break;
                    }
                }

                // V. Sort.
                int totalRecords = answerList.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                if (sort.ToUpper() == "DESC")
                {
                    answerList = answerList.OrderByDescending(t => t.Answer);
                    answerList = answerList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                else if (sort.ToUpper() == "ASC")
                {
                    answerList = answerList.OrderBy(t => t.Answer);
                    answerList = answerList.Skip(pageIndex * pageSize).Take(pageSize);
                }
                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = answerList
                };

                // VI.



                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<string> CreateAnswerInJqGrid([Bind(Exclude = "ID,QuestionId")] AnswerViewModel Model)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }

                if (!int.TryParse(Session["QuestionId"].ToString(), out int questionId))
                {
                    questionId = -1;
                }

                Model.QuestionId = questionId;
                if (ModelState.IsValid)
                {
                    AnswerDTO answerDTO = new AnswerDTO
                    {
                        QuestionId = Model.QuestionId,
                        AnswerText = Model.Answer,
                        IsProper = Model.IsProper
                    };
                    OperationDetails operationDetails = await AnswerService.CreateAsync(answerDTO, currentUserId);
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

        public async Task<string> EditAnswerInJqGrid(AnswerViewModel Model)
        {
            string msg;
            try
            {
                string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (currentUserId == null)
                {
                    return "User isn't authorized!";
                }
                if (ModelState.IsValid)
                {
                    AnswerDTO answerDTO = new AnswerDTO
                    {
                        AnswerId = Model.ID,
                        QuestionId = Model.QuestionId,
                        AnswerText = Model.Answer,
                        IsProper = Model.IsProper
                    };
                    OperationDetails operationDetails = await AnswerService.UpdateAsync(answerDTO, currentUserId);
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

        public async Task<string> DeleteAnswerInJqGrid(int id)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return "User isn't authorized!";
            }
            OperationDetails operationDetails = await AnswerService.DeleteAsync(id, currentUserId);
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