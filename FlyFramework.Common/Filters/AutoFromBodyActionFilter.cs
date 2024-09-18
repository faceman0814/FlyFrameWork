using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlyFramework.Common.Filters
{
    public class AutoFromBodyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.ContentType.Contains("application/json"))
            {
                var body = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
                object obj = JsonSerializer.Deserialize(body, context.ActionDescriptor.Parameters[0].ParameterType);
                context.ActionArguments[context.ActionDescriptor.Parameters[0].Name] = obj;
            }

            await next();
        }
    }
}
