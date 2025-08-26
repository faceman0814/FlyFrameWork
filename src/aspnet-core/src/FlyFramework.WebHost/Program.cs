using FaceMan.DynamicWebAPI.Config;
using FaceMan.DynamicWebAPI.Extensions;

using FlyFramework;
using FlyFramework.Extentions;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.Localizations;
using FlyFramework.Middlewares;

using Hangfire;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Minio;

using System;

var builder = WebApplication.CreateBuilder(args);

//配置注册服务并构建
var app = builder.ConfigurationServices().Build();
//启动配置服务管道
app.Configuration().Run();

/// <summary>
/// 配置类
/// </summary>
public static class AppConfig
{
    static WebApplicationBuilder builder;
    static WebApplication app;
    static IServiceCollection services;
    static IConfigurationRoot configuration;
    public static WebApplicationBuilder ConfigurationServices(this WebApplicationBuilder _builder)
    {
        builder = _builder;
        services = _builder.Services;

        var basePath = AppContext.BaseDirectory;
        configuration = new ConfigurationBuilder()
                       .SetBasePath(basePath)
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .Build();

        var _configParam = new SwaggerConfigParam
        {
            Title = "FlyFrameWork API",
            Version = "v1",
            Description = "FlyFrameWork API",
            ContactName = "FaceMan",
            EnableXmlComments = true,
            ApiDocsPath = "ApiDocs",
            EnableLoginPage = true,
            LoginPagePath = "pages/swagger.html",
            EnableApiResultFilter = true,
            ContactEmail = "face<EMAIL>",
            ContactUrl = "https://www.face-man.com",
            ApiRoutePrefix = "api",
            RoutePrefix = "swagger",
        };
        services.AddDynamicApi(builder.Environment.WebRootPath, _configParam);
        //可以注册其他自定义服务
        //_services.AddSingleton<Ixxx, xxx>();
        services.AddCors(configuration);

        services.AddHttpContextAccessor();

        // 添加响应压缩
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
        });

        // 添加内存缓存
        services.AddMemoryCache();

        // 添加分布式缓存（如果Redis启用）
        var redisConfig = configuration.GetSection("Redis");
        if (redisConfig.GetValue<bool>("Enable"))
        {
            services.AddDistributedMemoryCache(); // 可以替换为Redis实现
        }

        //services.AddAutoGnarly();


        //// 配置应用程序模块
        services.AddApplication<FlyFrameworkWebHostModule>();

        // 配置Autofac容器注入
        builder.Host.UseAutoFac();

        // 配置日志
        Log4Extention.InitLog4(builder.Logging);

        services.AddFilters();

        services.AddHangfire(configuration);

        services.AddRedis(configuration);

        services.AddMinio(configuration);

        //services.AddEventBus(configuration);

        services.AddRabbitMq(configuration);

        services.AddSignalR();

        // 配置JSON本地化
        services.AddJsonLocalization(options =>
        {
            options.ResourcesPath = "Localizations";

        }, typeof(FlyFrameworkWebHostModule));

        // 替换控制器激活器，使其支持通过Autofac容器进行注入
        builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
        return builder;
    }

    /// <summary>
    /// 启用服务集合
    /// </summary>
    /// <param name="_app"></param>
    /// <returns></returns>
    public static WebApplication Configuration(this WebApplication _app)
    {
        app = _app;
        //添加请求验证中间件
        app.UseMiddleware<RequestValidationMiddleware>();

        //添加性能监控中间件
        //app.UseMiddleware<PerformanceMonitoringMiddleware>();

        //添加全局异常处理中间件
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        // 配置CORS
        app.UseCors("DefaultCorsPolicy");

        // 配置国际化中间件
        app.UseRequestLocalization(options =>
        {
            var cultures = new[] { "zh-CN", "en-US", "zh-TW" };
            options.AddSupportedCultures(cultures);
            options.AddSupportedUICultures(cultures);
            options.SetDefaultCulture(cultures[0]);

            // 在Http响应时添加 当前语言信息 设置到 Response Header的Content-Language 中
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        app.UseRouting();
        app.UseDynamicSwagger();
        app.UseAuthentication(); //使用身份验证方式 身份验证中间件添加到管道中，这样在调用每个API时会自动执行身份验证
        app.UseIdentityServer();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        // 启用响应压缩
        app.UseResponseCompression();

        // 配置路由
        app.MapControllers();
        app.MapDefaultControllerRoute();
        //app.MapControllerRoute(
        //    name: "default",
        //    pattern: "{controller=Home}/{action=Index}/{id?}");
        //app.MapRazorPages();
        //配置 SignalR 端点
        //app.MapHub<SignalRTestHub>("/Hubs");
        if (configuration.GetSection("HangFire:Enable").Get<bool>())
        {
            // 配置Hangfire仪表板
            app.UseHangfireDashboard();
        }


        return app;
    }




}