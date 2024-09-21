using AutoMapper;

using DotNetCore.CAP.Internal;

using FlyFramework.Application.UserService.Mappers;
using FlyFramework.Common.Attributes;
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
using FlyFramework.Common.Utilities.Redis;
using FlyFramework.Core.RoleService;
using FlyFramework.Core.UserService;
using FlyFramework.EntityFrameworkCore;
using FlyFramework.EntityFrameworkCore.Extensions;
using FlyFramework.Repositories.Repositories;
using FlyFramework.Repositories.Uow;
using FlyFramework.WebHost.Controllers;
using FlyFramework.WebHost.Extentions;
using FlyFramework.WebHost.Filters;
using FlyFramework.WebHost.Identitys;

using Hangfire;
using Hangfire.MySql;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Minio;

using Newtonsoft.Json;

using ServiceStack.Redis;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
var builder = WebApplication.CreateBuilder(args);
// �����ļ���ȡ

//����ע����񲢹���
var app = builder.ConfigurationServices().Build();
//�������÷�������
app.Configuration().Run();

/// <summary>
/// ������
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
        var configuration = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();


        //��� cookie ��̬��
        //Cookies.serviceCollection = builder.Services;

        //����ע��ĳ�������������
        //_services.AddSingleton<Ixxx, xxx>();

        // ע��UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //hangfire������
        services.AddTransient<IMessageService, MessageService>();


        services.AddHttpContextAccessor();

        AddAutoMapper();

        AddJsonOptions();

        AddFilters();

        AddDbContext(configuration);

        AddHangfire(configuration);

        AddIdentity();

        AddJWT(configuration);

        AddDynamicApi();

        AddSwagger();

        AddAutoDI();

        AddRedis(configuration);

        AddMinio(configuration);

        AddEventBus(builder.Configuration);
        return builder;
    }
    public static void AddEventBus(IConfiguration configuration)
    {
        services.AddLocalEventBus();
        services.AddTransient<ILocalEventBus, MediatREventBus>();
        services.AddTransient<IDistributedEventBus, CapDistributedEventBus>();
        services.AddSingleton<IConsumerServiceSelector, FlyFrameworkConsumerServiceSelector>();
        services.AddCap(x =>
        {
            x.UseEntityFramework<FlyFrameworkDbContext>();

            x.UseSqlServer(configuration.GetConnectionString("Default"));
            //x.UseRabbitMQ(builder.Configuration["RabbitMq:Host"]);
            x.UseRabbitMQ(o => o.ConnectionFactoryOptions = factory =>
            {
                factory.Uri = new Uri(builder.Configuration["RabbitMq:Host"]);
            });
            //x.UseRedis(configuration["Cache:Redis"]);
        });
    }

    /// <summary>
    /// ����Hangfire
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="optionsAction"></param>
    public static void AddHangfire(IConfigurationRoot configuration, Action<BackgroundJobServerOptions> optionsAction = null)
    {
        // ��ȡ�����������
        var hangFireConfig = configuration.GetSection("HangFire").Get<HangFireOptionsConfig>();
        if (!hangFireConfig.Enable) return;

        var options = new BackgroundJobServerOptions()
        {
            ShutdownTimeout = TimeSpan.FromMinutes(30),
            Queues = new string[] { "default", "jobs" }, //�������ƣ�ֻ��ΪСд
            WorkerCount = 3, //Environment.ProcessorCount * 5, //���������� Math.Max(Environment.ProcessorCount, 20)
            ServerName = "fantasy.hangfire",
        };
        optionsAction?.Invoke(options);
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//��ǰ����
            .UseSimpleAssemblyNameTypeSerializer()//ʹ�ü򵥵ĳ��������������л���
            .UseRecommendedSerializerSettings()// ʹ���Ƽ������л�������
            .UseHangfireStorage(configuration)
        ).AddHangfireServer(optionsAction: c => c = options);
    }

    /// <summary>
    /// ����Minio
    /// </summary>
    /// <param name="configuration"></param>
    public static void AddMinio(IConfigurationRoot configuration)
    {
        // ��ȡ�����������
        var minioConfig = configuration.GetSection("Minio").Get<MinioOptionsConfig>();
        if (minioConfig.Enable)
        {
            var minioClient = new MinioClient()
               .WithEndpoint(minioConfig.EndPoint)
               .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
                .WithSSL(minioConfig.Secure)
                .Build();

            // ע��Redis�ͻ���ʵ��Ϊ��������
            services.AddSingleton(minioClient);

            // ע��Redis���湤��Ϊ��������
            services.AddSingleton<IMinioManager, MinioManager>();
        }


    }

    /// <summary>
    /// ����Redis����
    /// </summary>
    /// <param name="configuration"></param>
    public static void AddRedis(IConfigurationRoot configuration)
    {
        // ��ȡ�����������
        var cacheConfig = configuration.GetSection("Redis").Get<RedisOptionsConfig>();

        // �ж��Ƿ�����Redis����
        if (cacheConfig.Enable)
        {
            // ����Redis�����ַ���������
            var redisEndpoint = new RedisEndpoint()
            {
                Host = cacheConfig.Host, // Redis������ַ
                Port = cacheConfig.Port, // Redis�˿�
                Password = cacheConfig.Password, // Redis����
                Db = cacheConfig.Db, // Db�˿�
                //Ssl = cacheConfig.SSL // �Ƿ�����SSL
            };

            // ����Redis�ͻ���ʵ��
            var redis = new RedisClient(redisEndpoint);

            // ע��Redis�ͻ���ʵ��Ϊ��������
            services.AddSingleton<IRedisClient>(redis);

            // ע��Redis���湤��Ϊ��������
            services.AddSingleton<ICacheManager, RedisCacheManager>();
        }
        else
        {
            // ע���ڴ滺�����
            services.AddMemoryCache();

            // ע���ڴ滺�湤��Ϊ��������
            services.AddSingleton<ICacheManager, MemoryCacheManager>();

            // ע��ֲ�ʽ�ڴ滺�����
            services.AddDistributedMemoryCache();
        }
    }

    /// <summary>
    /// �����֤
    /// </summary>
    public static void AddIdentity()
    {
        services.AddIdentity<User, Role>()
       .AddEntityFrameworkStores<FlyFrameworkDbContext>()
        .AddDefaultTokenProviders();

        services.AddIdentityServer()
         .AddAspNetIdentity<User>()
         .AddDeveloperSigningCredential()
         //��չ��ÿ������ʱ��Ϊ����ǩ��������һ����ʱ��Կ�������ɻ�����Ҫһ���־û�����Կ
         .AddInMemoryClients(IdentityConfig.GetClients())             //��֤��ʽ
         .AddInMemoryApiResources(IdentityConfig.GetApiResources())
         .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())     //�����ӿڷ��ظ�ʽ
         .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
         .AddInMemoryPersistedGrants()
         .AddInMemoryCaching()
         .AddTestUsers(IdentityConfig.GetUsers());

    }

    /// <summary>
    /// JWT����
    /// </summary>
    /// <param name="config"></param>
    public static void AddJWT(IConfigurationRoot configuration)
    {
        //�������֤������ӵ��ܵ���
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
                //��֤Audience
                ValidateAudience = true,
                ValidAudience = jwtBearer.Audience,
                //��֤Issuer
                ValidateIssuer = true,
                ValidIssuer = jwtBearer.Issuer,
                //��֤ǩ��ʱ��
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                // ��֤ǩ��
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
                    var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "�ܱ�Ǹ������Ȩ���ʸýӿ�" });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.WriteAsync(payload);
                    return Task.CompletedTask;
                }
            };
        });
    }

    /// <summary>
    /// ���ö�̬API
    /// </summary>
    public static void AddDynamicApi()
    {
        services.AddMvc(options => { })
                .AddRazorPagesOptions((options) => { })
                .AddRazorRuntimeCompilation()
                .AddDynamicWebApi(builder.Configuration);

        services.AddSingleton<IJWTTokenManager, JWTTokenManager>();
        services.AddSingleton<IJwtBearerModel, JwtBearerModel>();
        //ע�ᶯ̬API����
        //services.AddControllers().AddDynamicWebApi(builder.Configuration);
    }

    /// <summary>
    /// ����Swagger
    /// </summary>
    public static void AddSwagger()
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            //�����Ӧͷ��Ϣ�������԰��������߲鿴 API ��Ӧ�а����� HTTP ͷ��Ϣ���Ӷ����õ���� API ����Ϊ��
            options.OperationFilter<AddResponseHeadersFilter>();
            //ժҪ�������Ȩ��Ϣ��������ÿ����Ҫ��Ȩ�Ĳ����Ա���ʾһ����ͼ�꣬���ѿ����߸ò�����Ҫ�����֤��
            options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            //�Ӱ�ȫ������Ϣ��������� API �İ�ȫ���ã��� OAuth2��JWT �ȣ��Զ�������Ӧ�İ�ȫ���������������������˽���Щ������Ҫ�ض��İ�ȫ���á�
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            options.DocumentFilter<RemoveAppFilter>();
            //ʹPost�����Body������Swagger UI����Json��ʽ��ʾ��
            options.OperationFilter<JsonBodyOperationFilter>();
            //����Զ����ĵ���Ϣ
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FlyFrameWork API",
                Version = "v1",
                Description = "FlyFrameWork API �ӿ��ĵ�",
                Contact = new OpenApiContact()
                {
                    Name = "FaceMan",
                    Email = "1002784867@qq.com",
                    Url = new Uri("https://github.com/faceman0814")
                }
            });

            //��������xml������
            var binXmlFiles =
                new DirectoryInfo(Path.Join(builder.Environment.WebRootPath, "ApiDocs"))
                    .GetFiles("*.xml", SearchOption.TopDirectoryOnly);
            foreach (var filePath in binXmlFiles.Select(item => item.FullName))
            {
                options.IncludeXmlComments(filePath, true);
            }

            //����AuthorizeȨ�ް�ť������ʽһ
            //options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
            //{
            //    Description = "���Ƿ�ʽһ(ֱ�����������������֤��Ϣ������Ҫ�ڿ�ͷ���Bearer) ",
            //    Name = "Authorization",        //jwtĬ�ϵĲ�������
            //    In = ParameterLocation.Header,  //jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
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
            ////����AuthorizeȨ�ް�ť������ʽ��

            //options.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme()
            //{
            //    Description = "���Ƿ�ʽ��(JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�)",
            //    Name = "Authorization",//jwtĬ�ϵĲ�������
            //    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
            //    Type = SecuritySchemeType.ApiKey
            //});

            ////����AuthorizeȨ�ް�ť����Ĭ��
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

            //����һ��Scheme��ע�������IdҪ������AddSecurityDefinition�еĲ���nameһ��
            //options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        { scheme, Array.Empty<string>() }
            //    });
        });
    }

    /// <summary>
    /// ����DbContext
    /// </summary>
    public static void AddDbContext(IConfigurationRoot configuration)
    {
        ////ע��DbContext����
        //services.AddDbContext<FlyFrameworkDbContext>(
        //    //option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
        //    option =>
        //    {
        //        option.UseSqlServer(builder.Configuration.GetConnectionString("default"));
        //        option.AddInterceptors(new FlyFrameworkInterceptor());
        //    }
        //);
        // ���DbContext����
        services.UsingDatabaseServices(configuration);
        //services.AddScoped<DbContext, FlyFrameworkDbContext>();

        //services.AddUnitOfWork<FlyFrameworkDbContext>(); // �����ݿ�֧��
        //ע�᷺�Ͳִ�����
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IDbContextProvider, DbContextProvider>();

    }

    /// <summary>
    /// �����Զ�ע������ע��
    /// </summary>
    public static void AddAutoDI()
    {
        services.AddDependencyServices();
        services.AddManagerRegisterServices();
    }

    /// <summary>
    /// ���ø�ʽ����Ӧ
    /// </summary>
    public static void AddJsonOptions()
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            //ʱ���ʽ����Ӧ
            options.JsonSerializerOptions.Converters.Add(new JsonOptionsDate("yyyy-MM-dd HH:mm:ss"));
            // ʹ��PascalCase������,��̬API�����õ�ֵ��
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //��ֹ�ַ�����ת���Unicode
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        });
    }

    /// <summary>
    /// ע��ȫ��������
    /// </summary>
    public static void AddFilters()
    {
        // ע��ȫ��������
        services.AddControllersWithViews(x =>
        {
            //ȫ�ַ��أ�ͳһ���ظ�ʽ
            x.Filters.Add<ApiResultFilterAttribute>();
            //ȫ������
            x.Filters.Add<UnitOfWorkFilter>();
            //������������
            x.Filters.Add<EnsureJsonFilterAttribute>();
            //����Post�����������json�����л���ֵ����
            x.Filters.Add(new AutoFromBodyActionFilter());
            //ȫ����־������
            //x.Filters.Add<LogAttribute>();
            //ȫ�������֤
            //x.Filters.Add<TokenAttribute>();
        });
    }

    /// <summary>
    ///���������С����
    /// </summary>
    public static void AddKestrel()
    {
        builder.WebHost.UseKestrel(options =>
        {
            options.Limits.MaxRequestLineSize = int.MaxValue;//HTTP �����е���������С�� Ĭ��Ϊ 8kb
            options.Limits.MaxRequestBufferSize = int.MaxValue;//���󻺳���������С�� Ĭ��Ϊ 1M
            //�κ��������ĵ���������С�����ֽ�Ϊ��λ��,Ĭ�� 30,000,000 �ֽڣ���ԼΪ 28.6MB
            options.Limits.MaxRequestBodySize = int.MaxValue;//�������󳤶�
        });

        /* �������������� ʹ��iis/nginx ������������ */
        services.Configure<FormOptions>(x =>
        {
            x.ValueCountLimit = 1000000; // ���ñ���ֵ�Ե��������
            x.ValueLengthLimit = int.MaxValue;// ���ñ����ݳ�������Ϊint�����ֵ
            x.MultipartBodyLengthLimit = int.MaxValue; // ���öಿ�����ĵĳ�������Ϊint�����ֵ
                                                       //x.MultipartHeadersCountLimit = 100; // ���öಿ�ֱ�ͷ���������
                                                       //x.MultipartHeadersLengthLimit = 16384; // ���öಿ�ֱ�ͷ����󳤶ȣ�bytes��
        });
    }

    /// <summary>
    /// ���ÿ���
    /// </summary>
    public static void AddCors()
    {
        services.AddCors(policy =>
        {
            /*
             * �����ڿ����������
             * [EnableCors("CorsPolicy")]
             */
            policy.AddPolicy("CorsPolicy", opt => opt
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            //#if !DEBUG
            //                .WithOrigins(WithOrigins)//����������
            //#endif
            .WithExposedHeaders("X-Pagination"));
        });
    }

    public static void AddAutoMapper()
    {
        // AutoMapper ����
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new UserMapper());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper); // ע�� IMapper �ӿ�
    }

    /// <summary>
    /// ʹ�� Hangfire Storage
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
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5), // ��������ҵ�����ʱʱ��Ϊ 5 ����
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // ��ҵ�Ŀɼ��Գ�ʱʱ��Ϊ 5 ����
                    QueuePollInterval = TimeSpan.FromSeconds(5), // �����ҵ���еļ��ʱ��Ϊ 5 ��
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),//- ��ҵ���ڼ������������ڼ�¼����Ĭ��ֵΪ1Сʱ��
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),//- �ۺϼ������ļ����Ĭ��Ϊ5���ӡ�
                    DashboardJobListLimit = 5000,//- �Ǳ����ҵ�б����ơ�Ĭ��ֵΪ50000��
                    TransactionTimeout = TimeSpan.FromMinutes(1),//- ���׳�ʱ��Ĭ��Ϊ1���ӡ�
                    UseRecommendedIsolationLevel = true, // ʹ���Ƽ���������뼶��
                    DisableGlobalLocks = true // ����ȫ����������
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
                    TablesPrefix = "FlyFramework_HangFire_"
                }));
                break;

            case DatabaseType.Psotgre:
                break;

            case DatabaseType.Sqlite:
                break;

            default:
                throw new Exception("��֧�ֵ����ݿ�����");
        }
        return globalConfiguration;
    }

    /// <summary>
    /// ���÷��񼯺�
    /// </summary>
    /// <param name="_app"></param>
    /// <returns></returns>
    public static WebApplication Configuration(this WebApplication _app)
    {
        app = _app;
        //app.UseMiddleware<UnitOfWorkMiddleware>();
        app.UseRouting();
        UseSwagger();
        app.UseAuthentication(); //ʹ����֤��ʽ �������֤�м����ӵ��ܵ��У���˽���ÿ�ε���APIʱ�Զ�ִ�������֤��
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
        // ����Hangfire�Ǳ���
        app.UseHangfireDashboard();
        return app;
    }

    /// <summary>
    /// ����Swagger
    /// </summary>
    public static void UseSwagger()
    {
        //������������Ի����ſ����ĵ���
        if (app.Environment.IsDevelopment() || app.Environment.IsTesting())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //����Endpoint·�����ĵ�����
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
                //����·��ǰ׺��RoutePrefix��Swagger UI�ĸ�·����
                //options.RoutePrefix = String.Empty;
                //����Ĭ��ģ��չ����ȡ�Ĭ��ֵΪ3���������ó�-1����ȫչ������ģ�͡�
                //options.DefaultModelExpandDepth(-1);
                // ���������ӹ��ܺ��û�����ֱ��ͨ��URL�����ض���API������ģ�ͣ�������Ҫ�ֶ���������Ӧ��λ�á�
                options.EnableDeepLinking();
                options.DocExpansion(DocExpansion.None); //swagger�ĵ�չ����ʽ��noneΪ�۵���listΪ�б�
                options.IndexStream = () =>
                {
                    var path = Path.Join(builder.Environment.WebRootPath, "pages", "swagger.html");
                    return new FileInfo(path).OpenRead();
                };
            });

        }
    }


}