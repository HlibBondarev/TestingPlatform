using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.AspNet.Identity;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;

[assembly: OwinStartup(typeof(SiteWithAuthentication.WEB.App_Start.Startup))]

namespace SiteWithAuthentication.WEB.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IUserService>(CreateUserService);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }

        private IUserService CreateUserService()
        {
            return new BLLUnitOfWork("DefaultConnection").UserService;
        }
    }
}
