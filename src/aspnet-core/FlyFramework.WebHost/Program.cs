using FlyFramework.Common.Extentions;
using FlyFramework.Common.FlyFrameworkModules.Extensions;
using FlyFramework.Domain.Localizations;
using FlyFramework.WebHost;
using FlyFramework.WebHost.Extentions;

using Hangfire;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Minio;

var builder = WebApplication.CreateBuilder(args);

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

        //单独注册某个服务，特殊情况
        //_services.AddSingleton<Ixxx, xxx>();

        services.AddHttpContextAccessor();

        //// 添加应用程序模块
        builder.Services.AddApplication<FlyFrameworkWebHostModule>();

        // 添加Autofac依赖注入
        builder.Host.UseAutoFac();

        // 配置日志
        builder.Host.ConfigureLogging((context, loggingBuilder) =>
        {
            Log4Extention.InitLog4(loggingBuilder);
        });

        services.AddJsonOptions();

        services.AddFilters();

        services.AddDbContext(configuration);

        services.AddHangfire(configuration);

        services.AddIdentity();

        services.AddJWT(configuration);

        services.AddDynamicApi(builder);

        services.AddSwagger(builder);

        services.AddDependencyServices();

        services.AddRedis(configuration);

        services.AddMinio(configuration);

        services.AddEventBus(configuration);

        services.AddRabbitMq(configuration);

        services.AddLocalCors(configuration);

        services.AddSignalR();

        // 添加JSON多语言
        services.AddJsonLocalization(options =>
        {
            options.ResourcesPath = "Localizations";

        }, typeof(FlyFrameworkWebHostModule));

        // 替换控制器构造器激活器以支持通过Autofac进行依赖注入
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

        // 启用中间件
        app.UseRequestLocalization(options =>
        {
            var cultures = new[] { "zh-CN", "en-US", "zh-TW" };
            options.AddSupportedCultures(cultures);
            options.AddSupportedUICultures(cultures);
            options.SetDefaultCulture(cultures[0]);

            // 当Http响应时，将 当前区域信息 设置到 Response Header：Content-Language 中
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        app.UseRouting();
        app.UseSwagger(builder);
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
            //添加 SignalR 端点
            //endpoints.MapHub<SignalRTestHub>("/Hubs");

        });

        if (configuration.GetSection("HangFire:Enable").Get<bool>())
        {
            // 启用Hangfire仪表盘
            app.UseHangfireDashboard();
        }


        return app;
    }




}