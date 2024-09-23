using FlyFramework.Application.Test;
using FlyFramework.Common.Extentions;
using FlyFramework.Repositories.Uow;
using FlyFramework.Repositories.UserSessions;
using FlyFramework.WebHost.Extentions;

using Hangfire;

using Minio;
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
        var configuration = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();

        // 配置日志
        builder.Host.ConfigureLogging((context, loggingBuilder) =>
        {
            Log4Extention.InitLog4(loggingBuilder);
        });

        //注入用户Session
        builder.Services.AddTransient<IUserSession, UserSession>();
        //单独注册某个服务，特殊情况
        //_services.AddSingleton<Ixxx, xxx>();
        // 注册UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //hangfire测试用
        services.AddTransient<IMessageService, MessageService>();

        services.AddHttpContextAccessor();

        services.AddAutoMapper();

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

        //app.UseMiddleware<UnitOfWorkMiddleware>();
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
        // 启用Hangfire仪表盘
        app.UseHangfireDashboard();
        return app;
    }




}