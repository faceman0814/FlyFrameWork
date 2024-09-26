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

        //����ע��ĳ�������������
        //_services.AddSingleton<Ixxx, xxx>();

        services.AddHttpContextAccessor();

        //// ���Ӧ�ó���ģ��
        builder.Services.AddApplication<FlyFrameworkWebHostModule>();

        // ���Autofac����ע��
        builder.Host.UseAutoFac();

        // ������־
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

        // ���JSON������
        services.AddJsonLocalization(options =>
        {
            options.ResourcesPath = "Localizations";

        }, typeof(FlyFrameworkWebHostModule));

        // �滻��������������������֧��ͨ��Autofac��������ע��
        builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
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

        // �����м��
        app.UseRequestLocalization(options =>
        {
            var cultures = new[] { "zh-CN", "en-US", "zh-TW" };
            options.AddSupportedCultures(cultures);
            options.AddSupportedUICultures(cultures);
            options.SetDefaultCulture(cultures[0]);

            // ��Http��Ӧʱ���� ��ǰ������Ϣ ���õ� Response Header��Content-Language ��
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

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

        if (configuration.GetSection("HangFire:Enable").Get<bool>())
        {
            // ����Hangfire�Ǳ���
            app.UseHangfireDashboard();
        }


        return app;
    }




}