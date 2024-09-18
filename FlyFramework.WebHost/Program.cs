using AutoMapper;

using FlyFramework.Application.UserService.Mappers;
using FlyFramework.Common.Extentions;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Common.Extentions.JsonOptions;
using FlyFramework.Common.Filters;
using FlyFramework.Common.Repositories;
using FlyFramework.Common.Uow;
using FlyFramework.Core.RoleService;
using FlyFramework.Core.UserService;
using FlyFramework.EntityFrameworkCore;
using FlyFramework.WebHost.Extentions;
using FlyFramework.WebHost.Identitys;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

using System.Text.Encodings.Web;
using System.Text.Unicode;
var builder = WebApplication.CreateBuilder(args);
// 配置文件读取
//var basePath = AppContext.BaseDirectory;
//var config = new ConfigurationBuilder()
//                .SetBasePath(basePath)
//                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                .Build();

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
        //添加 cookie 静态类
        //Cookies.serviceCollection = builder.Services;

        //单独注册某个服务，特殊情况
        //_services.AddSingleton<Ixxx, xxx>();
        // 注册UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        AddAutoMapper();

        AddJsonOptions();

        AddFilters();

        AddDbContext();

        AddDynamicApi();

        AddSwagger();

        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            //扩展在每次启动时，为令牌签名创建了一个临时密钥。在生成环境需要一个持久化的密钥
            .AddInMemoryClients(IdentityConfig.GetClients())             //验证方式
            .AddInMemoryApiResources(IdentityConfig.GetApiResources())
            .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())     //创建接口返回格式
            .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
            .AddInMemoryPersistedGrants()
            .AddInMemoryCaching()
            .AddTestUsers(IdentityConfig.GetUsers());                    //使用用户密码验证方式Ad
                                                                         //将身份验证服务添加到管道中
        services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "http://localhost:5134";   //你要请求验证的identity服务端的地址
            options.RequireHttpsMetadata = false;
            options.Audience = "api";          //你选择的验证方式。 对应的GetClients中定义的作用域
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });
        AddAutoDI();

        return builder;
    }

    /// <summary>
    /// 配置动态API
    /// </summary>
    public static void AddDynamicApi()
    {
        //注册动态API服务
        services.AddControllers().AddDynamicWebApi(builder.Configuration);
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
        services.AddIdentity<User, Role>().AddEntityFrameworkStores<FlyFrameworkDbContext>();
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
            x.Filters.Add<ResFilter>();
            //全局事务
            x.Filters.Add<UnitOfWorkFilter>();
            //配置请求类型
            x.Filters.Add<EnsureJsonFilter>();
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

        UseSwagger();
        app.UseAuthentication(); //使用验证方式 将身份认证中间件添加到管道中，因此将在每次调用API时自动执行身份验证。
        app.UseIdentityServer();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
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
                                                         //options.IndexStream = () =>
                                                         //{
                                                         //    var path = Path.Join(builder.Environment.WebRootPath, "pages", "swagger.html");
                                                         //    return new FileInfo(path).OpenRead();
                                                         //};
            });
        }
    }


}