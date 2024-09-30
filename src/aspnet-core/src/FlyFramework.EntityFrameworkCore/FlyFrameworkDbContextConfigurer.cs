using FlyFramework.Extensions;
using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Extensions;

using log4net;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace FlyFramework
{
    public static class FlyFrameworkDbContextConfigurer
    {
        private static readonly ILog log = LogManager.GetLogger("程序启动配置：");

        public static void UsingDatabaseServices(ServiceConfigerContext context)
        {
            var configuration = context.GetConfiguration();
            var databaseType = configuration.GetSection("ConnectionStrings:DatabaseType").Get<DatabaseType>();
            string connectionString = string.Empty;
            connectionString = configuration.GetSection("ConnectionStrings:Default").Get<string>();
            log.Info($"数据库类型：{databaseType}");
            log.Info($"连接字符串：{connectionString}");
            context.Services.AddDbContext<FlyFrameworkDbContext>(option =>
            {
                switch (databaseType)
                {
                    case DatabaseType.SqlServer:
                        option.UseSqlServer(connectionString).AddInterceptors(new QueryWithNoLockDbCommandInterceptor());
                        break;

                    case DatabaseType.MySql:
                        option.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31)));
                        break;

                    case DatabaseType.Sqlite:
                        option.UseSqlite(connectionString);
                        break;

                    case DatabaseType.Psotgre:
                        option.UseNpgsql(connectionString);
                        break;

                    //case DatabaseType.Oracle:
                    //    option.UseOracle(connectionString, (config) =>
                    //    {
                    //        config.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion23);
                    //        config.MigrationsHistoryTable("__EFMIGRATIONHISTORY");
                    //    }).AddInterceptors(new CommentCommandInterceptor())
                    //        //.UseRivenOracleTypeMapping()
                    //        ;
                    //    //if (DatabaseInfo.Instance.DatabaseType == DatabaseTypeEnum.Oracle)
                    //    //{
                    //    //    Configuration.UnitOfWork.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                    //    //}
                    //    break;

                    default:
                        throw new Exception("不支持的数据库类型");
                }
            });
        }
    }
}