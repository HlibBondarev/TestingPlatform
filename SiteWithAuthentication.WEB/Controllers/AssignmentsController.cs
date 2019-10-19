using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.WEB.Models;
using Microsoft.AspNet.Identity;

namespace SiteWithAuthentication.WEB.Controllers
{
    [Authorize(Roles = "admin, moderator")]
    public class AssignmentsController : Controller
    {
        private readonly IBLLUnitOfWork bLLUnitOfWork = new BLLUnitOfWork("DefaultConnection");    
        private ICommonService<CourseAssignmentDTO> CourseAssignmentService
        {
            get
            {
                return bLLUnitOfWork.CourseAssignmentService;
            }
        }

        // GET: Assignments/Index
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
                Session["ActiveController"] = "Assignments";

                // III. AutoMapper Setup.
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CourseDTO, CourseViewModel>()
                    .ForMember("Id", opt => opt.MapFrom(obj => obj.CourseId))
                    .ForMember("Name", opt => opt.MapFrom(obj => obj.CourseTitle));
                });
                IMapper iMapper = config.CreateMapper();

                // IV. Get data for a view.
                IEnumerable<CourseDTO> source = CourseAssignmentService
                    .Find(obj => obj.UserProfileId == currentUserId)
                    .Select(obj => obj.Course)
                    .OrderBy(obj => obj.CourseTitle);
                IEnumerable<CourseViewModel> courseOrderedList = iMapper.Map<IEnumerable<CourseDTO>, IEnumerable<CourseViewModel>>(source);

                // V.
                return View(courseOrderedList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}