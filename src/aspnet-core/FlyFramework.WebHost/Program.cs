using AutoMapper;

using FlyFramework.Application.UserService.Mappers;
using FlyFramework.Common.Attributes;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Common.Extentions.JsonOptions;
using FlyFramework.Common.Helpers.JWTTokens;
using FlyFramework.Common.Helpers.Minios;
using FlyFramework.Common.Helpers.Redis;
using FlyFramework.Core.RoleService;
using FlyFramework.Core.UserService;
using FlyFramework.EntityFrameworkCore;
using FlyFramework.Repositories.Repositories;
using FlyFramework.Repositories.Uow;
using FlyFramework.WebHost;
using FlyFramework.WebHost.Extentions;
using FlyFramework.WebHost.Filters;
using FlyFramework.WebHost.Identitys;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Minio;

using MongoDB.Driver;

using Newtonsoft.Json;

using ServiceStack.Redis;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

using System.Data.Common;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

using static ServiceStack.Diagnostics.Events;
var builder = WebApplication.CreateBuilder(args);
// 配置文件读取

//批量注册服务并构建
var app = builder.ConfigurationServices().Build();
//批量启用服务并运行
app.Configuration().Run();

/// <summary>
/// 配置类
/// </summary>
public static class AppConfig
{
    static WebApplicationBuilder builder;
    static WebApplication app;
    static IServiceCollection services;

    public static WebApplicationBuilder ConfigurationServices(this WebApplicationBuilder _builder)
    {
        builder = _builder;
        services = _builder.Services;

        var basePath = AppContext.BaseDirectory;
        var config = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();


        //添加 cookie 静态类
        //Cookies.serviceCollection = builder.Services;

        //单独注册某个服务，特殊情况
        //_services.AddSingleton<Ixxx, xxx>();

        // 注册UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpContextAccessor();

        AddAutoMapper();

        AddJsonOptions();

        AddFilters();

        AddDbContext();

        AddIdentity();

        AddJWT(config);

        AddDynamicApi();

        AddSwagger();

        AddAutoDI();

        AddRedis(config);

        AddMinio(config);

        return builder;
    }

    public static void AddMinio(IConfigurationRoot config)
    {
        // 获取缓存相关配置
        var minioConfig = config.GetSection("Minio").Get<MinioOptionsConfig>();
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
    public static void AddRedis(IConfigurationRoot config)
    {
        // 获取缓存相关配置
        var cacheConfig = config.GetSection("Redis").Get<RedisOptionsConfig>();

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
    public static void AddIdentity()
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
    public static void AddJWT(IConfigurationRoot config)
    {
        //将身份验证服务添加到管道中
        var jwtBearer = config.GetSection("JwtBearer").Get<JwtBearerModel>();
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
    public static void AddDynamicApi()
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
    public static void AddSwagger()
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
    public static void AddDbContext()
    {
        //注册DbContext服务
        services.AddDbContext<FlyFrameworkDbContext>(
            //option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
            option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("default"));
                option.AddInterceptors(new FlyFrameworkInterceptor());
            }
        );
        services.AddScoped<DbContext, FlyFrameworkDbContext>();

        //services.AddUnitOfWork<FlyFrameworkDbContext>(); // 多数据库支持
        //注册泛型仓储服务
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IDbContextProvider, DbContextProvider>();

    }

    /// <summary>
    /// 配置自动注册依赖注入
    /// </summary>
    public static void AddAutoDI()
    {
        services.AddDependencyServices();
        services.AddManagerRegisterServices();
    }

    /// <summary>
    /// 配置格式化响应
    /// </summary>
    public static void AddJsonOptions()
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
    public static void AddFilters()
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
    public static void AddKestrel()
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
    public static void AddCors()
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

    public static void AddAutoMapper()
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
    /// 启用服务集合
    /// </summary>
    /// <param name="_app"></param>
    /// <returns></returns>
    public static WebApplication Configuration(this WebApplication _app)
    {
        app = _app;
        //app.UseMiddleware<UnitOfWorkMiddleware>();
        app.UseRouting();
        UseSwagger();
        app.UseAuthentication(); //使用验证方式 将身份认证中间件添加到管道中，因此将在每次调用API时自动执行身份验证。
        app.UseIdentityServer();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            //endpoints.MapControllerRoute(
            //name: "default",
            //pattern: "{controller=Home}/{action=Index}/{id?}");
            //endpoints.MapRazorPages();
        });
        return app;
    }

    /// <summary>
    /// 启用Swagger
    /// </summary>
    public static void UseSwagger()
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