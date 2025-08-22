using FaceMan.DynamicWebAPI;
using FaceMan.DynamicWebAPI.Config;
using FaceMan.DynamicWebAPI.Extensions;

using FlyFramework;
using FlyFramework.Authorization;
using FlyFramework.Authorizations;
using FlyFramework.Extentions;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.Localizations;
using FlyFramework.OrgUnitModule.Authorization;
using FlyFramework.PermissionModule;

using Hangfire;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using FlyFramework.Models.Options;

using Minio;

using System;
using System.Collections.Generic;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
// Serilog bootstrap logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
//builder.Host.UseSerilog((context, services, configuration) =>
//{
//    var serilogSection = context.Configuration.GetSection("Serilog");
//    var writeToConsole = serilogSection.GetValue("WriteToConsole", true);
//    var writeToFile = serilogSection.GetValue("WriteToFile", true);
//    var filePath = serilogSection.GetValue("FilePath", "App_Data/Log/log-.txt");

//    configuration = configuration
//        .MinimumLevel.Information();
//    if (writeToConsole)
//    {
//        configuration = configuration.WriteTo.Console();
//    }
//    if (writeToFile)
//    {
//        configuration = configuration.WriteTo.File(filePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10);
//    }
//});

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
            ApiRoutePrefix="api",
            RoutePrefix="swagger",
        };
        services.AddDynamicApi(builder.Environment.WebRootPath, _configParam);
        //����ע��ĳ�������������
        //_services.AddSingleton<Ixxx, xxx>();
        services.AddCors(configuration);

        services.AddHttpContextAccessor();

        //services.AddAutoGnarly();


        //// ����Ӧ�ó���ģ��
        services.AddApplication<FlyFrameworkWebHostModule>();

        // ����Autofac����ע��
        builder.Host.UseAutoFac();

        // ������־
        // Log4 removed in favor of Serilog configuration above

        services.AddFilters();

        services.AddHangfire(configuration);

        services.AddRedis(configuration);

        services.AddMinio(configuration);

        //services.AddEventBus(configuration);

        services.AddRabbitMq(configuration);

        services.AddSignalR();

        // ����JSON������
        services.AddJsonLocalization(options =>
        {
            options.ResourcesPath = "Localizations";

        }, typeof(FlyFrameworkWebHostModule));

        // Options binding with validation
        services.AddOptions<JwtOptions>().Bind(builder.Configuration.GetSection("JwtBearer")).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<RedisOptions>().Bind(builder.Configuration.GetSection("Redis")).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<MinioOptions>().Bind(builder.Configuration.GetSection("Minio")).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<RabbitMqOptions>().Bind(builder.Configuration.GetSection("RabbitMq")).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<DatabaseOptions>().Bind(builder.Configuration.GetSection("ConnectionStrings")).ValidateDataAnnotations().ValidateOnStart();

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
        // ���ÿ���
        app.UseCors("DefaultCorsPolicy");
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
        app.UseDynamicSwagger();
        app.UseAuthentication(); //ʹ����֤��ʽ ��������֤�м�����ӵ��ܵ��У���˽���ÿ�ε���APIʱ�Զ�ִ��������֤��
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
            //���� SignalR �˵�
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