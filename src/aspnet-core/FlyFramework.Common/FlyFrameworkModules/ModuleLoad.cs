using FlyFramework.Common.FlyFrameworkModules.Extensions;
using FlyFramework.Common.FlyFrameworkModules.Interface;
using FlyFramework.Common.FlyFrameworkModules.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Common.FlyFrameworkModules
{
    public class ModuleLoad : IModuleLoad
    {
        public List<IBaseModuleDescritor> GetModuleDescritors(IServiceCollection service,
            Type startupModuleType)
        {
            service.CheckNull();
            List<IBaseModuleDescritor> result = new();
            LoadModules(startupModuleType, result, service);
            //反向排序 保证被依赖的模块优先级高于依赖的模块
            result.Reverse();
            return result;
        }

        protected virtual void LoadModules(Type type, List<IBaseModuleDescritor> descritors, IServiceCollection services)
        {
            foreach (var item in BaseModuleHelper.LoadModules(type))
            {
                descritors.Add(CreateModuleDescritor(item, services));
            }
        }

        protected virtual IBaseModule CreateAndRegisterModule(Type moduleType, IServiceCollection services)
        {
            var module = Activator.CreateInstance(moduleType) as IBaseModule;
            services.AddSingleton(moduleType, module);
            return module;
        }

        private IBaseModuleDescritor CreateModuleDescritor(Type type, IServiceCollection services)
        {
            return new BaseModuleDescritor(type, CreateAndRegisterModule(type, services));
        }
    }
}