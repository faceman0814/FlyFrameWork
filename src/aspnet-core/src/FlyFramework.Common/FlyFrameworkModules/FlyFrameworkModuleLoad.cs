using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Interface;
using FlyFramework.FlyFrameworkModules.Modules;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
namespace FlyFramework.FlyFrameworkModules
{
    public class FlyFrameworkModuleLoad : IFlyFrameworkModuleLoad
    {
        public List<IFlyFrameworkBaseModuleDescritor> GetModuleDescritors(IServiceCollection service,
            Type startupModuleType)
        {
            service.CheckNull();
            List<IFlyFrameworkBaseModuleDescritor> result = new();
            LoadModules(startupModuleType, result, service);
            //反向排序 保证被依赖的模块优先级高于依赖的模块
            result.Reverse();
            return result;
        }

        protected virtual void LoadModules(Type type, List<IFlyFrameworkBaseModuleDescritor> descritors, IServiceCollection services)
        {
            foreach (var item in FlyFrameworkBaseModuleHelper.LoadModules(type))
            {
                descritors.Add(CreateModuleDescritor(item, services));
            }
        }

        protected virtual IFlyFrameworkBaseModule CreateAndRegisterModule(Type moduleType, IServiceCollection services)
        {
            var module = Activator.CreateInstance(moduleType) as IFlyFrameworkBaseModule;
            services.AddSingleton(moduleType, module);
            return module;
        }

        private IFlyFrameworkBaseModuleDescritor CreateModuleDescritor(Type type, IServiceCollection services)
        {
            return new FlyFrameworkBaseModuleDescritor(type, CreateAndRegisterModule(type, services));
        }
    }
}