using Autofac;

using AutoMapper;

using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.Repositories;
using FlyFramework.Uow;
using FlyFramework.UserSessions;
using FlyFramework.Utilities.Dappers;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Data;

namespace FlyFramework
{
    [DependOn(
       typeof(FlyFrameworkApplicationModule)
   )]
    public class FlyFrameworkEntityFrameworkCoreModule : FlyFrameworkBaseModule
    {
        public override void PreInitialize(ServiceConfigerContext context)
        {
            FlyFrameworkDbContextConfigurer.UsingDatabaseServices(context);
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<,>))
                .As(typeof(IRepository<,>))
                .InstancePerLifetimeScope();

            builder.RegisterType<DbContextProvider>()
                  .As<IDbContextProvider>()
                  .InstancePerLifetimeScope();

            builder.Register<IDbConnection>(context =>
            {
                //var configuration = context.GetConfiguration();
                var configuration = context.Resolve<IConfiguration>();
                return new SqlConnection(configuration.GetConnectionString("Default"));
            }).InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(DapperManager<>))
                .As(typeof(IDapperManager<>))
              .InstancePerLifetimeScope();
            // 注册所有应用服务，并开启属性注入
            builder.RegisterAssemblyTypes(typeof(FlyFrameworkEntityFrameworkCoreModule).Assembly)
                   //.Where(t => t.Name.EndsWith("AppService"))
                   //.EnableClassInterceptors() // 如果使用拦截器
                   .PropertiesAutowired(); // 启用属性注入
        }
    }
}