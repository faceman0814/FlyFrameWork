using Autofac;

using FlyFramework.Common.Dependencys;
using FlyFramework.Domain.ApplicationServices;
using FlyFramework.Repositories.UserSessions;

using System.IdentityModel.Tokens.Jwt;

using Module = Autofac.Module;

namespace FlyFramework.WebHost.Autofac
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder container)
        {
            Type baseType = typeof(IDependency);
            var basePath = AppContext.BaseDirectory;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName.StartsWith("FlyFramework."));

            foreach (var assembly in assemblies)
            {
                container.RegisterAssemblyTypes(assembly)
                    .Where(b => !b.IsAbstract)
                    .AsImplementedInterfaces();
            }

            // 用于Jwt的各种操作
            container.RegisterType<JwtSecurityTokenHandler>().InstancePerLifetimeScope();

            container.RegisterType<UserSession>().As<IUserSession>().InstancePerLifetimeScope();
            container.RegisterType<ApplicationService>().As<IApplicationService>().PropertiesAutowired().InstancePerLifetimeScope();
        }
    }
}