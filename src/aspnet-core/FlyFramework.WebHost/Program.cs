using FlyFramework.Application.Test;
using FlyFramework.Common.Extentions;
using FlyFramework.Repositories.Uow;
using FlyFramework.Repositories.UserSessions;
using FlyFramework.WebHost.Extentions;

using Hangfire;

using Minio;
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

        // ������־
        builder.Host.ConfigureLogging((context, loggingBuilder) =>
        {
            Log4Extention.InitLog4(loggingBuilder);
        });

        //ע���û�Session
        builder.Services.AddTransient<IUserSession, UserSession>();
        //����ע��ĳ�������������
        //_services.AddSingleton<Ixxx, xxx>();
        // ע��UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //hangfire������
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
    /// ���÷��񼯺�
    /// </summary>
    /// <param name="_app"></param>
    /// <returns></returns>
    public static WebApplication Configuration(this WebApplication _app)
    {
        app = _app;

        //app.UseMiddleware<UnitOfWorkMiddleware>();
        app.UseRouting();
        app.UseSwagger(builder);
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
            //��� SignalR �˵�
            //endpoints.MapHub<SignalRTestHub>("/Hubs");

        });
        // ����Hangfire�Ǳ���
        app.UseHangfireDashboard();
        return app;
    }




}