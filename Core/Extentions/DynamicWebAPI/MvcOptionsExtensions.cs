using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extentions.DynamicWebAPI
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// 扩展方法
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="routeAttribute"></param>
        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            //添加我们自定义实现
            opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }
    }
}
