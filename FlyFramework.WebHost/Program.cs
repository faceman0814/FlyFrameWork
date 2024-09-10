using FlyFramework.Core.Extentions.DynamicWebAPI;

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
    //options.DocumentFilter<RemoveAppSuffixFilter>();
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
    // ���� XML �ĵ�ע��
    //var xmlPath = Path.Combine($"{builder.Environment.WebRootPath}", "ApiDoc.xml");
    //options.IncludeXmlComments(xmlPath, true);

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
//ע�ᶯ̬API����
builder.Services.AddControllers().AddDynamicWebApi(builder.Configuration);

#endregion ConfigurationServices



#region Configuration
var app = builder.Build();

app.UseRouting();
//���������Ͳ��Ի����ſ����ĵ���
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
        //options.RoutePrefix = String.Empty;
        //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        //options.DefaultModelExpandDepth(-1);
        //options.EnableDeepLinking(); //�����ӹ���
        options.DocExpansion(DocExpansion.None); //swagger�ĵ��Ƿ��
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