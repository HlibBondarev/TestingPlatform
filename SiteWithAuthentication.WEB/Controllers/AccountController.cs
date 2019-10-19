using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Security.Claims;
using SiteWithAuthentication.WEB.Models;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Infrastructure;

namespace SiteWithAuthentication.WEB.Controllers
{
    public class AccountController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            //await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserProfileDTO userDto = new UserProfileDTO { Email = model.Email, Password = model.Password };
                ClaimsIdentity claim = await UserService.AuthenticateAsync(userDto);
                if (claim == null)
                {
                    ModelState.AddModelError("", "Wrong login or password.");
                }
                else
                {
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (Request.QueryString.GetValues("ReturnUrl") != null)
                    {
                        string backRef = Request.QueryString.GetValues("ReturnUrl")[0];
                        // Return to the back reference.
                        return Redirect(backRef);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegistryViewModel model)
        {
            //await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserProfileDTO userDto = new UserProfileDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    Address = model.Address,
                    UserName = model.Name,
                    Role = "user"
                };
                OperationDetails operationDetails = await UserService.CreateAsync(userDto);
                if (operationDetails.Succedeed)
                    return View("SuccessRegister");
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }

        //private async Task SetInitialDataAsync()
        //{
        //    // This code initializes Admin.
        //    await UserService.SetInitialDataAsync(new UserProfileDTO
        //    {
        //        Email = "somemail@mail.ru",
        //        UserName = "JohnDow",
        //        Password = "ad46D_ewr3",
        //        Address = "ул. Спортивная, д.30, кв.75",
        //        Role = "admin",
        //        // This code initializes role list.
        //    }, new List<string> { "user", "moderator", "admin" });
        //}
    }
}