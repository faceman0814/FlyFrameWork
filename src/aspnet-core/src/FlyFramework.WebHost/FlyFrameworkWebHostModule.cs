using Autofac;

using FlyFramework.Authorizations;
using FlyFramework.Controllers;
using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.PermissionModule;
using FlyFramework.Repositories;
using FlyFramework.Uow;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkWebCoreModule))]
    public class FlyFrameworkWebHostModule : FlyFrameworkBaseModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HomeController>().InstancePerLifetimeScope();
            builder.RegisterType<AccountClientController>().InstancePerLifetimeScope();
        }

        //public override void PostInitialize(ServiceConfigerContext context)
        //{
        //    // 手动创建权限定义上下文
        //    var permissionContext = new PermissionDefinitionContext(context.Provider);

        //    // 创建一个事务
        //    using (var uowManager = context.Provider.GetService<IUnitOfWorkManager>())
        //    {
        //        using (var uow = uowManager.Begin())
        //        {
        //            var context1 = uowManager.Current.GetDbContext();
        //            // 创建并执行权限提供者
        //            var authorizationProvider = new FlyFrameworkAuthorizationProvider();
        //            authorizationProvider.SetPermissions(permissionContext);
        //            uow.SaveChangesAsync();
        //        }
        //    }
        //    // 注册权限提供者
        //}
    }
}
