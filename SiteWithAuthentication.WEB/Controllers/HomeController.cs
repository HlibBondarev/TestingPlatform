using System.Collections.Generic;
using System.Web.Mvc;
using SiteWithAuthentication.WEB.Util.SiteMenu;
using Microsoft.AspNet.Identity;
using SiteWithAuthentication.BLL.Interfaces;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.BLL.DTO;

namespace SiteWithAuthentication.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBLLUnitOfWork bLLUnitOfWork = new BLLUnitOfWork("DefaultConnection");
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        private ICommonService<SubscriptionForModeratorDTO> SubscriptionForModeratorService
        {
            get
            {
                return bLLUnitOfWork.SubscriptionForModeratorService;
            }
        }

        public ActionResult Menu()
        {
            string roleName = string.Empty;
            MenuList menuList = new MenuList();
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (currentUserId != null)
            {
                roleName = UserService.FindUserRoleById(currentUserId);
            }
            else
            {
                roleName = null;
            }
            IEnumerable<MenuItem> menu = menuList.GetMenu(roleName);
            return PartialView(menu);
        }

        public ActionResult Index()
        {
            return View();
        }

        //[Authorize]
        public ActionResult About()
        {
            return View();
        }

        //[Authorize]
        public ActionResult Contact()
        {
            return View();
        }
    }
}