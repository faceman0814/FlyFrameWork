using Core.Extentions.DynamicWebAPI;

using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddDynamicWebApi(builder.Configuration);
var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
