using FlyFramework.Application.Extentions.DynamicWebAPI;
using FlyFramework.WebCore.Extentions;

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
    //options.DocumentFilter<RemoveAppSuffixFilter>();
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
//注册动态API服务
builder.Services.AddControllers().AddDynamicWebApi(builder.Configuration);

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