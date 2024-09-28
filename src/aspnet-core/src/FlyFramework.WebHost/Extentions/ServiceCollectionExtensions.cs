using Autofac;
using Autofac.Extensions.DependencyInjection;

using DotNetCore.CAP.Internal;

using FlyFramework.Attributes;
using FlyFramework.DynamicWebAPI;
using FlyFramework.Extensions;
using FlyFramework.Extentions;
using FlyFramework.Extentions.JsonOptions;
using FlyFramework.Filters;
using FlyFramework.Identitys;
using FlyFramework.Repositories;
using FlyFramework.RoleService;
using FlyFramework.UserService;
using FlyFramework.Utilities.Dappers;
using FlyFramework.Utilities.EventBus;
using FlyFramework.Utilities.EventBus.Distributed;
using FlyFramework.Utilities.EventBus.Distributed.Cap;
using FlyFramework.Utilities.EventBus.Local;
using FlyFramework.Utilities.EventBus.MediatR;
using FlyFramework.Utilities.HangFires;
using FlyFramework.Utilities.JWTTokens;
using FlyFramework.Utilities.Minios;
using FlyFramework.Utilities.RabbitMqs;
using FlyFramework.Utilities.Redis;

using Hangfire;
using Hangfire.MySql;
using Hangfire.SqlServer;

using log4net;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Minio;

using MongoDB.Driver;

using Newtonsoft.Json;

using RabbitMQ.Client;

using ServiceStack;
using ServiceStack.Redis;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
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
        /// 身份验证
        /// </summary>
        public static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
           .AddEntityFrameworkStores<FlyFrameworkDbContext>()
            .AddDefaultTokenProviders();

            services.AddIdentityServer()
             .AddAspNetIdentity<User>()
             .AddDeveloperSigningCredential()
             //扩展在每次启动时，为令牌签名创建了一个临时密钥。在生成环境需要一个持久化的密钥
             .AddInMemoryClients(IdentityConfig.GetClients())             //验证方式
             .AddInMemoryApiResources(IdentityConfig.GetApiResources())
             .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())     //创建接口返回格式
             .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
             .AddInMemoryPersistedGrants()
             .AddInMemoryCaching()
             .AddTestUsers(IdentityConfig.GetUsers());

        }

        /// <summary>
        /// JWT配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddJWT(this IServiceCollection services, IConfigurationRoot configuration)
        {
            //将身份验证服务添加到管道中
            var jwtBearer = configuration.GetSection("JwtBearer").Get<JwtBearerModel>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "BearerCookie";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(jwtBearer.AccessTokenExpiresMinutes);
                options.SlidingExpiration = false;
                options.LogoutPath = "/Home/Index";
                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningOut = async context =>
                    {
                        context.Response.Cookies.Delete("access-token");
                        await Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //验证Audience
                    ValidateAudience = true,
                    ValidAudience = jwtBearer.Audience,
                    //验证Issuer
                    ValidateIssuer = true,
                    ValidIssuer = jwtBearer.Issuer,
                    //验证签发时间
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    // 验证签名
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearer.SecretKey)),
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "很抱歉，您无权访问该接口" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.WriteAsync(payload);
                        return Task.CompletedTask;
                    }
                };
            });
        }

        /// <summary>
        /// 配置动态API
        /// </summary>
        public static void AddDynamicApi(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddMvc(options => { })
                    .AddRazorPagesOptions((options) => { })
                    .AddRazorRuntimeCompilation()
                    .AddDynamicWebApi(builder.Configuration);

            //services.AddSingleton<IJWTTokenManager, JWTTokenManager>();
            services.AddSingleton<IJwtBearerModel, JwtBearerModel>();
        }

        /// <summary>
        /// 配置Swagger
        /// </summary>
        public static void AddSwagger(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                //添加响应头信息。它可以帮助开发者查看 API 响应中包含的 HTTP 头信息，从而更好地理解 API 的行为。
                options.OperationFilter<AddResponseHeadersFilter>();
                //摘要中添加授权信息。它会在每个需要授权的操作旁边显示一个锁图标，提醒开发者该操作需要身份验证。
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //加安全需求信息。它会根据 API 的安全配置（如 OAuth2、JWT 等）自动生成相应的安全需求描述，帮助开发者了解哪些操作需要特定的安全配置。
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.DocumentFilter<RemoveAppFilter>();
                //使Post请求的Body参数在Swagger UI中以Json格式显示。
                options.OperationFilter<JsonBodyOperationFilter>();
                options.OperationFilter<WrapResponseOperationFilter>(); // 添加自定义Swagger操作过滤器
                //添加自定义文档信息
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FlyFrameWork API",
                    Version = "v1",
                    Description = "FlyFrameWork API 接口文档",
                    Contact = new OpenApiContact()
                    {
                        Name = "FaceMan",
                        Email = "1002784867@qq.com",
                        Url = new Uri("https://github.com/faceman0814")
                    }
                });

                //遍历所有xml并加载
                var binXmlFiles =
                    new DirectoryInfo(Path.Join(builder.Environment.WebRootPath, "ApiDocs"))
                        .GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                foreach (var filePath in binXmlFiles.Select(item => item.FullName))
                {
                    options.IncludeXmlComments(filePath, true);
                }

                //开启Authorize权限按钮——方式一
                //options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
                //{
                //    Description = "这是方式一(直接在输入框中输入认证信息，不需要在开头添加Bearer) ",
                //    Name = "Authorization",        //jwt默认的参数名称
                //    In = ParameterLocation.Header,  //jwt默认存放Authorization信息的位置(请求头中)
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "Bearer"
                //});
                //var scheme = new OpenApiSecurityScheme
                //{
                //    Reference = new OpenApiReference()
                //    {
                //        Id = "JWTBearer",
                //        Type = ReferenceType.SecurityScheme
                //    }
                //};
                ////开启Authorize权限按钮——方式二

                //options.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme()
                //{
                //    Description = "这是方式二(JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）)",
                //    Name = "Authorization",//jwt默认的参数名称
                //    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                //    Type = SecuritySchemeType.ApiKey
                //});

                ////开启Authorize权限按钮——默认
                //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            },Scheme = "oauth2",Name = "Bearer",In=ParameterLocation.Header,
                //        },new List<string>()
                //    }
                //});

                //声明一个Scheme，注意下面的Id要和上面AddSecurityDefinition中的参数name一致
                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //    {
                //        { scheme, Array.Empty<string>() }
                //    });
            });
        }

        /// <summary>
        /// 配置DbContext
        /// </summary>
        public static void AddDbContext(this IServiceCollection services, IConfigurationRoot configuration)
        {
            // 添加DbContext服务
            services.UsingDatabaseServices(configuration, log);
            //注册泛型仓储服务
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IDbContextProvider, DbContextProvider>();
            // 注册IDbConnection，使用Scoped生命周期
            services.AddScoped<IDbConnection>(provider =>
                new SqlConnection(configuration.GetConnectionString("Default")));
            services.AddScoped(typeof(IDapperManager<>), typeof(DapperManager<>));
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
        /// 注册全局拦截器
        /// </summary>
        public static void AddFilters(this IServiceCollection services)
        {
            // 注册全局拦截器
            services.AddControllersWithViews(x =>
            {
                //全局返回，异常处理，统一返回格式。
                x.Filters.Add<ApiResultFilterAttribute>();
                //全局事务
                x.Filters.Add<UnitOfWorkFilter>();
                //配置请求类型
                //x.Filters.Add<EnsureJsonFilterAttribute>();
                //解析Post请求参数，将json反序列化赋值参数
                x.Filters.Add(new AutoFromBodyActionFilter());
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
                containerBuilder.RegisterModule(new FlyFrameworkApplicationModule());
                containerBuilder.RegisterModule(new FlyFrameworkCoreModule());
                containerBuilder.RegisterModule(new FlyFrameworkWebHostModule());
            });
            return hostBuilder;
        }


        public static void AddLocalizations(this IServiceCollection services)
        {

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

                case DatabaseType.Psotgre:
                    break;

                case DatabaseType.Sqlite:
                    break;

                default:
                    throw new Exception("不支持的数据库类型");
            }
            return globalConfiguration;
        }

        /// <summary>
        /// 启用Swagger
        /// </summary>
        public static void UseSwagger(this WebApplication app, WebApplicationBuilder builder)
        {
            //开发环境或测试环境才开启文档。
            if (app.Environment.IsDevelopment() || app.Environment.IsTesting())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    //配置Endpoint路径和文档标题
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
                    //配置路由前缀，RoutePrefix是Swagger UI的根路径。
                    //options.RoutePrefix = String.Empty;
                    //设置默认模型展开深度。默认值为3，可以设置成-1以完全展开所有模型。
                    //options.DefaultModelExpandDepth(-1);
                    // 启用深链接功能后，用户可以直接通过URL访问特定的API操作或模型，而不需要手动导航到相应的位置。
                    options.EnableDeepLinking();
                    options.DocExpansion(DocExpansion.None); //swagger文档展开方式，none为折叠，list为列表
                    options.IndexStream = () =>
                    {
                        var path = Path.Join(builder.Environment.WebRootPath, "pages", "swagger.html");
                        return new FileInfo(path).OpenRead();
                    };
                });

            }
        }
    }

}
