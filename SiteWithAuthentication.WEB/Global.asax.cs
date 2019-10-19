using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.WEB.Util;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SiteWithAuthentication.BLL.Util;
using AutoMapper;


namespace SiteWithAuthentication.WEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Dependency injection.
            NinjectModule userModule = new UserModule();
            NinjectModule serviceModule = new ServiceModule("DefaultConnection");

            var kernel = new StandardKernel(userModule, serviceModule);
            // Because of Ninject CLR throws this exeption.
            // Exception Details: 
            // System.InvalidOperationException: 
            // Validation type names in unobtrusive client validation rules must be unique. The following validation 
            // type was seen more than once: regex.
            // So I add this code.
            kernel.Unbind<ModelValidatorProvider>();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));

            // Initialize AutoMapper.
            //IMapper mapper = BLLAutoMapper.GetMapper;
        }
    }
}
