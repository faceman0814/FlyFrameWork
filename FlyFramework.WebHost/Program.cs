using FlyFramework.Core.Extentions.DynamicWebAPI;

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
    // 启用 XML 文档注释
    //var xmlPath = Path.Combine($"{builder.Environment.WebRootPath}", "ApiDoc.xml");
    //options.IncludeXmlComments(xmlPath, true);

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

app.UseRouting();
//开发环境和测试环境才开启文档。
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
        //options.RoutePrefix = String.Empty;
        //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        //options.DefaultModelExpandDepth(-1);
        //options.EnableDeepLinking(); //深链接功能
        options.DocExpansion(DocExpansion.None); //swagger文档是否打开
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