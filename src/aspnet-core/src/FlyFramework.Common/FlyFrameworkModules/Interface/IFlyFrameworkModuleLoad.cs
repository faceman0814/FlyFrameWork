using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleLoad
    {
        List<IFlyFrameworkBaseModuleDescritor> GetModuleDescritors(IServiceCollection service, Type startupModuleType);
    }
}