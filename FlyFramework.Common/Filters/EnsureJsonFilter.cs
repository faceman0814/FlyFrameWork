﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Filters
{
    public class EnsureJsonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                // 强制将ContentType设置为 application/json
                if (!request.ContentType.Contains("application/json"))
                {
                    context.Result = new BadRequestObjectResult("Content type 'application/json' is required for this endpoint.");
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
