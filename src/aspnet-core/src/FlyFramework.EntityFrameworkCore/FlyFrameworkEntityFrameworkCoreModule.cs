using Autofac;

using AutoMapper;

using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.Localizations;
using FlyFramework.Repositories;
using FlyFramework.Seed;
using FlyFramework.Uow;
using FlyFramework.UserSessions;
using FlyFramework.Utilities.Dappers;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
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

            context.Services.AddTransient<IDbContextProvider, DbContextProvider>();
            context.Services.AddTransient<IDatabaseChecker, DatabaseChecker<FlyFrameworkDbContext>>();
            context.Services.AddTransient<IUnitOfWorkManager, UnitOfWorkManager>();
        }
        public override void Initialize(ServiceConfigerContext context)
        {

        }
        public override void PostInitialize(ServiceConfigerContext context)
        {
            //获取配置访问器：
            var configuration = context.Provider.GetRequiredService<IConfiguration>();
            //创建作用域：
            using (var scope = context.Provider.CreateScope())
            {
                //获取数据库连接字符串：
                var connectionString = configuration.GetConnectionString("Default");
                //检查数据库是否存在：
                var databaseChecker = scope.ServiceProvider.GetRequiredService<IDatabaseChecker>();
                var dbExist = databaseChecker.Exist(connectionString);

                if (!FlyFrameworkConfigs.Database.SkipDbSeed && dbExist)
                {
                    SeedHelper.SeedHostDb(context.Provider);
                }
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            // 注册数据库检查器
            builder.RegisterGeneric(typeof(DatabaseChecker<>))
                    .As(typeof(IDatabaseChecker<>))
                    .InstancePerLifetimeScope();

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