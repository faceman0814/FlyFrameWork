using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.Repositories;
using FlyFramework.Utilities.Dappers;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Data;

namespace FlyFramework
{
    [DependOn(
       typeof(FlyFrameworkCoreModule)
   )]
    public class FlyFrameworkEntityFrameworkCoreModule : FlyFrameworkBaseModule
    {
        public override void PreInitialize(ServiceConfigerContext context)
        {
            FlyFrameworkDbContextConfigurer.UsingDatabaseServices(context);


        }

        public override void Initialize(ServiceConfigerContext context)
        {
            var configuration = context.GetConfiguration();
            //注册泛型仓储服务
            context.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            context.Services.AddScoped<IDbContextProvider, DbContextProvider>();
            // 注册IDbConnection，使用Scoped生命周期
            context.Services.AddScoped<IDbConnection>(provider =>
                new SqlConnection(configuration.GetConnectionString("Default")));
            context.Services.AddScoped(typeof(IDapperManager<>), typeof(DapperManager<>));
        }
    }
}