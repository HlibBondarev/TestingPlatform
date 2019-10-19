using Ninject.Modules;
using SiteWithAuthentication.DAL.Interfaces;
using SiteWithAuthentication.DAL.Repositories;

namespace SiteWithAuthentication.BLL.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        private readonly string connectionString;
        public ServiceModule(string connection)
        {
            connectionString = connection;
        }
        public override void Load()
        {
            Bind<IUnitOfWork>().To<EFUnitOfWork>().WithConstructorArgument(connectionString);
        }
    }
}
