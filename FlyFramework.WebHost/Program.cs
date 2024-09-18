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
// �����ļ���ȡ
//var basePath = AppContext.BaseDirectory;
//var config = new ConfigurationBuilder()
//                .SetBasePath(basePath)
//                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                .Build();

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
        //��� cookie ��̬��
        //Cookies.serviceCollection = builder.Services;

        //����ע��ĳ�������������
        //_services.AddSingleton<Ixxx, xxx>();
        // ע��UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        AddAutoMapper();

        AddJsonOptions();

        AddFilters();

        AddDbContext();

        AddDynamicApi();

        AddSwagger();

        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            //��չ��ÿ������ʱ��Ϊ����ǩ��������һ����ʱ��Կ�������ɻ�����Ҫһ���־û�����Կ
            .AddInMemoryClients(IdentityConfig.GetClients())             //��֤��ʽ
            .AddInMemoryApiResources(IdentityConfig.GetApiResources())
            .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())     //�����ӿڷ��ظ�ʽ
            .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
            .AddInMemoryPersistedGrants()
            .AddInMemoryCaching()
            .AddTestUsers(IdentityConfig.GetUsers());                    //ʹ���û�������֤��ʽAd
                                                                         //�������֤������ӵ��ܵ���
        services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "http://localhost:5134";   //��Ҫ������֤��identity����˵ĵ�ַ
            options.RequireHttpsMetadata = false;
            options.Audience = "api";          //��ѡ�����֤��ʽ�� ��Ӧ��GetClients�ж����������
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });
        AddAutoDI();

        return builder;
    }

    /// <summary>
    /// ���ö�̬API
    /// </summary>
    public static void AddDynamicApi()
    {
        //ע�ᶯ̬API����
        services.AddControllers().AddDynamicWebApi(builder.Configuration);
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
        });
    }

    /// <summary>
    /// ����DbContext
    /// </summary>
    public static void AddDbContext()
    {
        //ע��DbContext����
        services.AddDbContext<FlyFrameworkDbContext>(
            //option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
            option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("default"));
                option.AddInterceptors(new FlyFrameworkInterceptor());
            }
        );
        services.AddScoped<DbContext, FlyFrameworkDbContext>();

        //services.AddUnitOfWork<FlyFrameworkDbContext>(); // �����ݿ�֧��
        //ע�᷺�Ͳִ�����
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IDbContextProvider, DbContextProvider>();
        services.AddIdentity<User, Role>().AddEntityFrameworkStores<FlyFrameworkDbContext>();
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
            x.Filters.Add<ResFilter>();
            //ȫ������
            x.Filters.Add<UnitOfWorkFilter>();
            //������������
            x.Filters.Add<EnsureJsonFilter>();
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
    /// ���÷��񼯺�
    /// </summary>
    /// <param name="_app"></param>
    /// <returns></returns>
    public static WebApplication Configuration(this WebApplication _app)
    {
        app = _app;
        //app.UseMiddleware<UnitOfWorkMiddleware>();

        UseSwagger();
        app.UseAuthentication(); //ʹ����֤��ʽ �������֤�м����ӵ��ܵ��У���˽���ÿ�ε���APIʱ�Զ�ִ�������֤��
        app.UseIdentityServer();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
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
                                                         //options.IndexStream = () =>
                                                         //{
                                                         //    var path = Path.Join(builder.Environment.WebRootPath, "pages", "swagger.html");
                                                         //    return new FileInfo(path).OpenRead();
                                                         //};
            });
        }
    }


}