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
// �����ļ���ȡ
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

#region ConfigurationServices

#region ���Swagger�ĵ�����

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //�����Ӧͷ��Ϣ�������԰��������߲鿴 API ��Ӧ�а����� HTTP ͷ��Ϣ���Ӷ����õ���� API ����Ϊ��
    options.OperationFilter<AddResponseHeadersFilter>();
    //ժҪ�������Ȩ��Ϣ��������ÿ����Ҫ��Ȩ�Ĳ����Ա���ʾһ����ͼ�꣬���ѿ����߸ò�����Ҫ�����֤��
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    //�Ӱ�ȫ������Ϣ��������� API �İ�ȫ���ã��� OAuth2��JWT �ȣ��Զ�������Ӧ�İ�ȫ���������������������˽���Щ������Ҫ�ض��İ�ȫ���á�
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.DocumentFilter<RemoveAppFilter>();
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

#endregion ���Swagger�ĵ�����

#region ע�ᶯ̬API����
//ע�ᶯ̬API����
builder.Services.AddControllers().AddDynamicWebApi(builder.Configuration);
#endregion ע�ᶯ̬API����

#region ע��ִ�����
//ע��DbContext����
string connectionString = builder.Configuration.GetConnectionString("default");

builder.Services.AddDbContext<FlyFrameworkDbContext>(
    //option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
    option => option.UseSqlServer(connectionString)
);
builder.Services.AddScoped<DbContext, FlyFrameworkDbContext>();
// ע�Ṥ����Ԫ
builder.Services.AddUnitOfWork();
//builder.Services.AddUnitOfWork<MyDbContext>(); // �����ݿ�֧��
//ע�᷺�Ͳִ�����
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

#endregion ע��ִ�����

#region ��Ӧ�ӿ�ע������ע�����

// ��ȡ��ǰӦ�ó��������Ѽ��ص��� "FlyFramework" ��ͷ�ĳ���
var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(t => t.FullName.StartsWith("FlyFramework")).ToArray();

// �������������ĳ���
foreach (var assembly in assemblies)
{
    Console.WriteLine("��������: " + assembly.FullName);
    // ɨ����������зǳ���������
    var types = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract);

    foreach (var type in types)
    {
        var interfaces = type.GetInterfaces();
        var dependencyInterfaces = interfaces.Intersect(new[] { typeof(ITransientDependency), typeof(IScopedDependency), typeof(ISingletonDependency) });

        if (!dependencyInterfaces.Any()) continue;

        // �������������������ӿڲ�ע�ᵽ����������
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

app.UseHttpsRedirection();
// ���������֤
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion