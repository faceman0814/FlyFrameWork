using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlyFramework.Common.Filters
{
    public class RemoveAppFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var newTags = new List<OpenApiTag>();
            // 去掉控制器分类中的 "App" 后缀
            foreach (var path in swaggerDoc.Paths.Values)
            {
                foreach (var operation in path.Operations.Values)
                {
                    if (operation.Tags.Any(tag => tag.Name.EndsWith("AppService", StringComparison.OrdinalIgnoreCase)))
                    {
                        var tags = operation.Tags.Select(tag => new OpenApiTag
                        {
                            Name = tag.Name.Replace("AppService", "", StringComparison.OrdinalIgnoreCase),
                            Description = tag.Description
                        }).ToList();

                        operation.Tags.Clear();
                        foreach (var tag in tags)
                        {
                            operation.Tags.Add(tag);
                        }
                    }
                }
            }
        }
    }
}
