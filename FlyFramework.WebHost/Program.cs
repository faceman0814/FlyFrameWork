using EntityFrameworkCore.Repository;
using EntityFrameworkCore.Repository.Interfaces;
using EntityFrameworkCore.UnitOfWork.Extensions;

using FlyFramework.Application.Extentions.DynamicWebAPI;
using FlyFramework.Common.Dependencys;
using FlyFramework.EntityFrameworkCore;
using FlyFramework.WebCore.Extentions;
using FlyFramework.WebCore.Filters;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
// 配置文件读取
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

#region ConfigurationServices

#region 添加Swagger文档服务

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //添加响应头信息。它可以帮助开发者查看 API 响应中包含的 HTTP 头信息，从而更好地理解 API 的行为。
    options.OperationFilter<AddResponseHeadersFilter>();
    //摘要中添加授权信息。它会在每个需要授权的操作旁边显示一个锁图标，提醒开发者该操作需要身份验证。
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    //加安全需求信息。它会根据 API 的安全配置（如 OAuth2、JWT 等）自动生成相应的安全需求描述，帮助开发者了解哪些操作需要特定的安全配置。
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.DocumentFilter<RemoveAppFilter>();
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

#endregion 添加Swagger文档服务

#region 注册动态API服务
//注册动态API服务
builder.Services.AddControllers().AddDynamicWebApi(builder.Configuration);
#endregion 注册动态API服务

#region 注册仓储服务
//注册DbContext服务
string connectionString = builder.Configuration.GetConnectionString("default");

builder.Services.AddDbContext<FlyFrameworkDbContext>(
    //option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
    option => option.UseSqlServer(connectionString)
);
builder.Services.AddScoped<DbContext, FlyFrameworkDbContext>();
// 注册工作单元
builder.Services.AddUnitOfWork();
//builder.Services.AddUnitOfWork<MyDbContext>(); // 多数据库支持
//注册泛型仓储服务
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

#endregion 注册仓储服务

#region 对应接口注册依赖注入服务

// 获取当前应用程序域中已加载的以 "FlyFramework" 开头的程序集
var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(t => t.FullName.StartsWith("FlyFramework")).ToArray();

// 遍历符合条件的程序集
foreach (var assembly in assemblies)
{
    Console.WriteLine("程序集名称: " + assembly.FullName);
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
                Console.WriteLine(type.AssemblyQualifiedName);
                builder.Services.AddTransient(serviceType, type);
            }
            else if (typeof(IScopedDependency).IsAssignableFrom(serviceType))
            {
                builder.Services.AddScoped(serviceType, type);
            }
            else if (typeof(ISingletonDependency).IsAssignableFrom(serviceType))
            {
                builder.Services.AddSingleton(serviceType, type);
            }
        }
    }
}

#endregion

#endregion ConfigurationServices



#region Configuration
var app = builder.Build();

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

app.UseHttpsRedirection();
// 启用身份验证
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion