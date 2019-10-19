using Ninject.Modules;
using SiteWithAuthentication.BLL.Services;
using SiteWithAuthentication.BLL.Interfaces;


namespace SiteWithAuthentication.WEB.Util
{
    public class UserModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserService>().To<UserService>();
        }
    }
}