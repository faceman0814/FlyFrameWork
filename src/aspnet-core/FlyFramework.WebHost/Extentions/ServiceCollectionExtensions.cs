using AutoMapper;

using DotNetCore.CAP.Internal;

using FlyFramework.Application.UserService.Mappers;
using FlyFramework.Common.Attributes;
using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Common.Extentions.JsonOptions;
using FlyFramework.Common.Utilities.EventBus;
using FlyFramework.Common.Utilities.EventBus.Distributed;
using FlyFramework.Common.Utilities.EventBus.Distributed.Cap;
using FlyFramework.Common.Utilities.EventBus.Local;
using FlyFramework.Common.Utilities.EventBus.MediatR;
using FlyFramework.Common.Utilities.HangFires;
using FlyFramework.Common.Utilities.JWTTokens;
using FlyFramework.Common.Utilities.Minios;
using FlyFramework.Common.Utilities.RabbitMqs;
using FlyFramework.Common.Utilities.Redis;
using FlyFramework.Core.RoleService;
using FlyFramework.Core.UserService;
using FlyFramework.EntityFrameworkCore;
using FlyFramework.EntityFrameworkCore.Extensions;
using FlyFramework.Repositories.Repositories;
using FlyFramework.WebHost.Filters;
using FlyFramework.WebHost.Identitys;

using Hangfire;
using Hangfire.MySql;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Minio;

using Newtonsoft.Json;

using RabbitMQ.Client;

using ServiceStack;
using ServiceStack.Redis;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
namespace FlyFramework.WebHost.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyServices(this IServiceCollection services)
        {
            // 获取当前应用程序域中已加载的以 "FlyFramework" 开头的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith("FlyFramework")).ToArray();

            // 遍历符合条件的程序集
            foreach (var assembly in assemblies)
            {
                // 扫描程序集中所有非抽象类类型
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract);

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();
                    var dependencyInterfaces = interfaces.Intersect(new[] { typeof(ITransientDependency), typeof(IScopedDependency), typeof(ISingletonDependency) });

                    if (!dependencyInterfaces.Any()) continue;

                    // 遍历符合条件的依赖接口并注册到服务容器中
                    foreach (var serviceType in dependencyInterfaces)
                    {
                        if (typeof(ITransientDependency).IsAssignableFrom(serviceType))
                        {
                            services.AddTransient(serviceType, type);
                        }
                        else if (typeof(IScopedDependency).IsAssignableFrom(serviceType))
                        {
                            services.AddScoped(serviceType, type);
                        }
                        else if (typeof(ISingletonDependency).IsAssignableFrom(serviceType))
                        {
                            services.AddSingleton(serviceType, type);
                        }
                    }
                }
            }
            return services;
        }

        public static IServiceCollection AddManagerRegisterServices(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
               .Where(t => t.FullName.StartsWith("FlyFramework"))
               .ToArray();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract);

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces().Where(t => t.Name.EndsWith("Manager")).Distinct();

                    // 自动注册与接口名称匹配的服务实现
                    foreach (var interfaceType in interfaces)
                    {
                        if (!interfaceType.IsPublic || interfaceType == typeof(IDisposable))
                            continue;

                        if (interfaceType.Name.Equals($"I{type.Name}", StringComparison.OrdinalIgnoreCase))
                        {
                            RegisterService(services, interfaceType, type);
                        }
                    }
                }
            }

            return services;
        }

        private static void RegisterService(IServiceCollection services, Type interfaceType, Type implementationType)
        {
            // 默认注册为 Scoped，可根据需要调整
            services.AddScoped(interfaceType, implementationType);
        }

        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            // 获取配置
            var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqOptionsConfig>();
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqConfig.HostName,
                Port = 5672,
                UserName = rabbitMqConfig.UserName,
                Password = rabbitMqConfig.Password
            };

            // 注册到依赖注入系统
            services.AddSingleton<IConnectionFactory>(_ => factory);
            services.AddSingleton<IRabbitMqManager, RabbitMqManager>();
        }


        public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            // 获取缓存相关配置
            var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqOptionsConfig>();
            services.AddLocalEventBus();
            services.AddTransient<ILocalEventBus, MediatREventBus>();
            services.AddTransient<IDistributedEventBus, CapDistributedEventBus>();
            services.AddSingleton<IConsumerServiceSelector, FlyFrameworkConsumerServiceSelector>();
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

        /// <summary>
        /// 配置Hangfire
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="optionsAction"></param>
        public static void AddHangfire(this IServiceCollection services, IConfigurationRoot configuration, Action<BackgroundJobServerOptions> optionsAction = null)
        {
            // 获取缓存相关配置
            var hangFireConfig = configuration.GetSection("HangFire").Get<HangFireOptionsConfig>();
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
            if (minioConfig.Enable)
            {
                var minioClient = new MinioClient()
                   .WithEndpoint(minioConfig.EndPoint)
                   .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
                    .WithSSL(minioConfig.Secure)
                    .Build();

                // 注册Redis客户端实例为单例服务
                services.AddSingleton(minioClient);

                // 注册Redis缓存工具为单例服务
                services.AddSingleton<IMinioManager, MinioManager>();
            }


        }

        /// <summary>
        /// 配置Redis缓存
        /// </summary>
        /// <param name="configuration"></param>
        public static void AddRedis(this IServiceCollection services, IConfigurationRoot configuration)
        {
            // 获取缓存相关配置
            var cacheConfig = configuration.GetSection("Redis").Get<RedisOptionsConfig>();

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
        /// <param name="config"></param>
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

            services.AddSingleton<IJWTTokenManager, JWTTokenManager>();
            services.AddSingleton<IJwtBearerModel, JwtBearerModel>();
            //注册动态API服务
            //services.AddControllers().AddDynamicWebApi(builder.Configuration);
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
            ////注册DbContext服务
            //services.AddDbContext<FlyFrameworkDbContext>(
            //    //option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
            //    option =>
            //    {
            //        option.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            //        option.AddInterceptors(new FlyFrameworkInterceptor());
            //    }
            //);
            // 添加DbContext服务
            services.UsingDatabaseServices(configuration);
            //services.AddScoped<DbContext, FlyFrameworkDbContext>();

            //services.AddUnitOfWork<FlyFrameworkDbContext>(); // 多数据库支持
            //注册泛型仓储服务
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IDbContextProvider, DbContextProvider>();

        }

        /// <summary>
        /// 配置自动注册依赖注入
        /// </summary>
        public static void AddAutoDI(this IServiceCollection services)
        {
            services.AddDependencyServices();
            services.AddManagerRegisterServices();
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
                //全局返回，统一返回格式
                x.Filters.Add<ApiResultFilterAttribute>();
                //全局事务
                x.Filters.Add<UnitOfWorkFilter>();
                //配置请求类型
                x.Filters.Add<EnsureJsonFilterAttribute>();
                //解析Post请求参数，将json反序列化赋值参数
                x.Filters.Add(new AutoFromBodyActionFilter());
                //全局日志，报错
                //x.Filters.Add<LogAttribute>();
                //全局身份验证
                //x.Filters.Add<TokenAttribute>();
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
        /// 配置跨域
        /// </summary>
        public static void AddCors(this IServiceCollection services)
        {
            services.AddCors(policy =>
            {
                /*
                 * 可以在控制器处添加
                 * [EnableCors("CorsPolicy")]
                 */
                policy.AddPolicy("CorsPolicy", opt => opt
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                //#if !DEBUG
                //                .WithOrigins(WithOrigins)//域名白名单
                //#endif
                .WithExposedHeaders("X-Pagination"));
            });
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            // AutoMapper 配置
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMapper());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper); // 注册 IMapper 接口
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
