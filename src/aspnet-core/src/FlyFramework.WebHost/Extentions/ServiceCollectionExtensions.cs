using Autofac;
using Autofac.Extensions.DependencyInjection;

using DotNetCore.CAP.Internal;

using FlyFramework.Attributes;
using FlyFramework.Extensions;
using FlyFramework.Extentions;
using FlyFramework.Extentions.JsonOptions;
using FlyFramework.Filters;
using FlyFramework.Repositories;
using FlyFramework.Utilities.EventBus;
using FlyFramework.Utilities.EventBus.Distributed;
using FlyFramework.Utilities.EventBus.Distributed.Cap;
using FlyFramework.Utilities.EventBus.Local;
using FlyFramework.Utilities.EventBus.MediatR;
using FlyFramework.Utilities.HangFires;
using FlyFramework.Utilities.Minios;
using FlyFramework.Utilities.RabbitMqs;
using FlyFramework.Utilities.Redis;

using Hangfire;
using Hangfire.MySql;
using Hangfire.SqlServer;

using log4net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Minio;

using MongoDB.Driver;

using RabbitMQ.Client;

using ServiceStack;
using ServiceStack.Redis;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
namespace FlyFramework.Extentions
{
    public static class ServiceCollectionExtensions
    {
        private const string DefaultCorsPolicyName = "DefaultCorsPolicy";

        public static string[] InterfacePostfixes { get; set; } = { "Manager", "AppService", "Service" };

        private static readonly ILog log = LogManager.GetLogger("程序启动配置：");

        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            // 获取配置
            var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqOptionsConfig>();
            log.Info($"RabbitMq:{rabbitMqConfig.Enable},Host:{rabbitMqConfig.HostName}:{rabbitMqConfig.Port}");
            ConnectionFactory factory = default;
            if (rabbitMqConfig.Enable)
            {
                factory = new ConnectionFactory
                {
                    HostName = rabbitMqConfig.HostName,
                    Port = 5672,
                    UserName = rabbitMqConfig.UserName,
                    Password = rabbitMqConfig.Password
                };
            }
            // 注册到依赖注入系统
            services.AddSingleton<IConnectionFactory>(_ => factory);
        }


        public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalEventBus();
            services.AddTransient<ILocalEventBus, MediatREventBus>();
            var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqOptionsConfig>();
            if (rabbitMqConfig.Enable)
            {
                services.AddCap(x =>
                {
                    x.UseEntityFramework<FlyFrameworkDbContext>();
                    //x.UseSqlServer(configuration.GetConnectionString("Default"));
                    //x.UseRabbitMQ(builder.Configuration["RabbitMq:Host"]);
                    x.UseRabbitMQ(o => o.ConnectionFactoryOptions = factory =>
                    {
                        factory.Uri = new Uri("amqp://" + rabbitMqConfig.UserName + ":" + rabbitMqConfig.Password + "@" + rabbitMqConfig.HostName + ":" + rabbitMqConfig.Port);
                    });
                    //x.UseRedis(configuration["Cache:Redis"]);
                });
            }
            else
            {
                return;
            }
            services.AddTransient<IDistributedEventBus, CapDistributedEventBus>();
            services.AddSingleton<IConsumerServiceSelector, FlyFrameworkConsumerServiceSelector>();
        }

        /// <summary>
        /// 配置Hangfire
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="optionsAction"></param>
        public static void AddHangfire(this IServiceCollection services, IConfigurationRoot configuration, Action<BackgroundJobServerOptions> optionsAction = null)
        {
            // 获取缓存相关配置
            var hangFireConfig = configuration.GetSection("HangFire").Get<HangFireOptionsConfig>();
            log.Info($"HangFire:{hangFireConfig.Enable}");
            if (hangFireConfig.Enable)
            {
                var options = new BackgroundJobServerOptions()
                {
                    ShutdownTimeout = TimeSpan.FromMinutes(30),
                    Queues = new string[] { "default", "jobs" }, //队列名称，只能为小写
                    WorkerCount = 3, //Environment.ProcessorCount * 5, //并发任务数 Math.Max(Environment.ProcessorCount, 20)
                    ServerName = "fantasy.hangfire",
                };
                optionsAction?.Invoke(options);
                services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//向前兼容
                    .UseSimpleAssemblyNameTypeSerializer()//使用简单的程序集名称类型序列化器
                    .UseRecommendedSerializerSettings()// 使用推荐的序列化器设置
                    .UseHangfireStorage(configuration)
                ).AddHangfireServer(optionsAction: c => c = options);
            }
        }

        /// <summary>
        /// 配置Minio
        /// </summary>
        /// <param name="configuration"></param>
        public static void AddMinio(this IServiceCollection services, IConfigurationRoot configuration)
        {
            // 获取缓存相关配置
            var minioConfig = configuration.GetSection("Minio").Get<MinioOptionsConfig>();
            log.Info($"Minio:{minioConfig.Enable},Host:{minioConfig.EndPoint}");
            IMinioClient minioClient = new MinioClient();
            if (minioConfig.Enable)
            {
                minioClient = new MinioClient()
                  .WithEndpoint(minioConfig.EndPoint)
                  .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
                   .WithSSL(minioConfig.Secure)
                   .Build();

                // 注册Redis客户端实例为单例服务
                services.AddSingleton(minioClient);

                // 注册Redis缓存工具为单例服务
            }
            services.AddSingleton(minioClient);
        }

        /// <summary>
        /// 配置Redis缓存
        /// </summary>
        /// <param name="configuration"></param>
        public static void AddRedis(this IServiceCollection services, IConfigurationRoot configuration)
        {
            // 获取缓存相关配置
            var cacheConfig = configuration.GetSection("Redis").Get<RedisOptionsConfig>();
            log.Info($"Redis:{cacheConfig.Enable},Host:{cacheConfig.Host}:{cacheConfig.Port}");
            // 判断是否启用Redis缓存
            if (cacheConfig.Enable)
            {
                // 创建Redis连接字符串构建器
                var redisEndpoint = new RedisEndpoint()
                {
                    Host = cacheConfig.Host, // Redis主机地址
                    Port = cacheConfig.Port, // Redis端口
                    Password = cacheConfig.Password, // Redis密码
                    Db = cacheConfig.Db, // Db端口
                                         //Ssl = cacheConfig.SSL // 是否启用SSL
                };

                // 创建Redis客户端实例
                var redis = new RedisClient(redisEndpoint);

                // 注册Redis客户端实例为单例服务
                services.AddSingleton<IRedisClient>(redis);

                // 注册Redis缓存工具为单例服务
                services.AddSingleton<ICacheManager, RedisCacheManager>();
            }
            else
            {
                // 注册内存缓存服务
                services.AddMemoryCache();

                // 注册内存缓存工具为单例服务
                services.AddSingleton<ICacheManager, MemoryCacheManager>();

                // 注册分布式内存缓存服务
                services.AddDistributedMemoryCache();
            }
        }

        /// <summary>
        /// 配置格式化响应
        /// </summary>
        public static void AddJsonOptions(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                //时间格式化响应
                options.JsonSerializerOptions.Converters.Add(new JsonOptionsDate("yyyy-MM-dd HH:mm:ss"));
                // 使用PascalCase属性名,动态API才能拿到值。
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //禁止字符串被转义成Unicode
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

            });
        }

        /// <summary>
        /// 配置全局过滤器
        /// </summary>
        /// <param name="services"></param>
        public static void AddFilters(this IServiceCollection services)
        {
            services.AddControllersWithViews(x =>
            {
                //全局事务
                x.Filters.Add<UnitOfWorkFilter>();
                //接口返回
                //x.Filters.Add<ApiResultFilterAttribute>();
            });
        }

        /// <summary>
        ///配置请求大小限制
        /// </summary>
        public static void AddKestrel(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.WebHost.UseKestrel(options =>
            {
                options.Limits.MaxRequestLineSize = int.MaxValue;//HTTP 请求行的最大允许大小。 默认为 8kb
                options.Limits.MaxRequestBufferSize = int.MaxValue;//请求缓冲区的最大大小。 默认为 1M
                                                                   //任何请求正文的最大允许大小（以字节为单位）,默认 30,000,000 字节，大约为 28.6MB
                options.Limits.MaxRequestBodySize = int.MaxValue;//限制请求长度
            });

            /* ↓↓↓↓↓↓↓ 使用iis/nginx ↓↓↓↓↓↓ */
            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = 1000000; // 设置表单键值对的最大数量
                x.ValueLengthLimit = int.MaxValue;// 设置表单数据长度限制为int的最大值
                x.MultipartBodyLengthLimit = int.MaxValue; // 设置多部分正文的长度限制为int的最大值
                                                           //x.MultipartHeadersCountLimit = 100; // 设置多部分表单头的最大数量
                                                           //x.MultipartHeadersLengthLimit = 16384; // 设置多部分表单头的最大长度（bytes）
            });
        }

        /// <summary>
        /// CORS策略
        /// </summary>
        /// <returns></returns>
        public static void AddCors(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var corsOrigins = configuration["CorsOrigins"]
               .Split(",", StringSplitOptions.RemoveEmptyEntries)
               .Select(o => o.RemovePostFix("/"))
               .Distinct()
               .ToArray();

            services.AddCors(
                options => options.AddPolicy(
                    DefaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(corsOrigins)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                .AllowCredentials()
                )
            );
        }

        /// <summary>
        /// AutoFac 配置
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder UseAutoFac(this IHostBuilder hostBuilder)
        {
            // 使用 Autofac 作为服务提供器工厂
            hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // 配置 Autofac 特有的依赖注入
            hostBuilder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                // 注册自定义的 Autofac 模块
                containerBuilder.RegisterModule(new FlyFrameworkCommonModule());
                containerBuilder.RegisterModule(new FlyFrameworkDomainModule());
                containerBuilder.RegisterModule(new FlyFrameworkRepositoriesModule());
                containerBuilder.RegisterModule(new FlyFrameworkCoreModule());
                containerBuilder.RegisterModule(new FlyFrameworkApplicationModule());
                containerBuilder.RegisterModule(new FlyFrameworkEntityFrameworkCoreModule());
                containerBuilder.RegisterModule(new FlyFrameworkWebHostModule());
            });
            return hostBuilder;
        }


        public static void AddDynamicRepositories(this IServiceCollection services)
        {
            //扫描继承了IEntity的接口
            //注册IEntity接口的实现类
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => t.IsInterface);
                foreach (var type in types)
                {
                    var entityType = type.GetGenericArguments().FirstOrDefault();
                    if (entityType == null) continue;
                    var repositoryType = typeof(IEntity<>).MakeGenericType(type, entityType);
                    services.AddTransient(type, repositoryType);
                }
            }
        }

        /// <summary>
        /// 使用 Hangfire Storage
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IGlobalConfiguration UseHangfireStorage(this IGlobalConfiguration globalConfiguration, IConfigurationRoot configuration)
        {
            var databaseType = configuration.GetSection("ConnectionStrings:DatabaseType").Get<DatabaseType>();
            var connectionString = configuration.GetSection("ConnectionStrings:Default").Get<string>();
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    globalConfiguration.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        SchemaName = "FlyFramework_HangFire_",
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5), // 批处理作业的最大超时时间为 5 分钟
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // 作业的可见性超时时间为 5 分钟
                        QueuePollInterval = TimeSpan.FromSeconds(5), // 检查作业队列的间隔时间为 5 秒
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),//- 作业到期检查间隔（管理过期记录）。默认值为1小时。
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),//- 聚合计数器的间隔。默认为5分钟。
                        DashboardJobListLimit = 5000,//- 仪表板作业列表限制。默认值为50000。
                        TransactionTimeout = TimeSpan.FromMinutes(1),//- 交易超时。默认为1分钟。
                        UseRecommendedIsolationLevel = true, // 使用推荐的事务隔离级别
                        DisableGlobalLocks = true // 禁用全局锁定机制
                    });
                    break;

                case DatabaseType.MySql:
                    globalConfiguration.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions()
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "FlyFramework_HangFire"
                    }));
                    break;

                case DatabaseType.Postgre:
                    break;

                case DatabaseType.Sqlite:
                    break;

                default:
                    throw new Exception("不支持的数据库类型");
            }
            return globalConfiguration;
        }
    }
}
