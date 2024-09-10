using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
namespace Core.Extentions.DynamicWebAPI
{
    /// <summary>
    /// 自定义应用程序模型约定，用于配置实现了 IApplicationService 接口的控制器。
    /// </summary>
    public class ApplicationServiceConvention : IApplicationModelConvention
    {
        private IConfiguration _configuration;
        private List<HttpMethodConfigure> httpMethods = new();
        public ApplicationServiceConvention(IConfiguration configuration)
        {
            _configuration = configuration;
            httpMethods = _configuration.GetSection("HttpMethodInfo").Get<List<HttpMethodConfigure>>();
        }
        public void Apply(ApplicationModel application)
        {
            //循环每一个控制器信息
            foreach (var controller in application.Controllers)
            {
                var controllerType = controller.ControllerType.AsType();
                //是否继承IApplicationService接口
                if (typeof(IApplicationService).IsAssignableFrom(controllerType))
                {
                    foreach (var item in controller.Actions)
                    {
                        ConfigureSelector(controller.ControllerName, item);
                    }
                }
            }
        }

        private void ConfigureSelector(string controllerName, ActionModel action)
        {
            for (int i = 0; i < action.Selectors.Count; i++)
            {
                if (action.Selectors[i].AttributeRouteModel is null)
                    action.Selectors.Remove(action.Selectors[i]);
            }

            if (action.Selectors.Any())
            {
                foreach (var item in action.Selectors)
                {
                    var routePath = string.Concat("api/", controllerName + "/", action.ActionName).Replace("//", "/");
                    var routeModel = new AttributeRouteModel(new RouteAttribute(routePath));
                    //如果没有路由属性
                    if (item.AttributeRouteModel == null) item.AttributeRouteModel = routeModel;
                }
            }
            else
            {
                action.Selectors.Add(CreateActionSelector(controllerName, action));
            }
        }

        private SelectorModel CreateActionSelector(string controllerName, ActionModel action)
        {
            var selectorModel = new SelectorModel();
            var actionName = action.ActionName;
            string httpMethod = string.Empty;
            //是否有HttpMethodAttribute
            var routeAttributes = action.ActionMethod.GetCustomAttributes(typeof(HttpMethodAttribute), false);
            //如果标记了HttpMethodAttribute
            if (routeAttributes != null && routeAttributes.Any())
            {
                httpMethod = routeAttributes.SelectMany(m => (m as HttpMethodAttribute).HttpMethods).ToList().Distinct().FirstOrDefault();
            }
            else
            {
                var methodName = action.ActionMethod.Name.ToUpper();

                foreach (var item in httpMethods)
                {
                    foreach (var method in item.MethodVal)
                    {
                        if (methodName.StartsWith(method))
                        {
                            httpMethod = item.MethodKey;
                            break;
                        }

                    }
                }
                if (httpMethod == string.Empty)
                {
                    httpMethod = "POST";
                }
            }
            return ConfigureSelectorModel(selectorModel, action, controllerName, httpMethod);
        }

        public SelectorModel ConfigureSelectorModel(SelectorModel selectorModel, ActionModel action, string controllerName, string httpMethod)
        {
            var routePath = string.Concat("api/", controllerName + "/", action.ActionName).Replace("//", "/");
            //给此Action添加路由
            selectorModel.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routePath));
            //添加HttpMethod
            selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
            return selectorModel;
        }

    }


}
