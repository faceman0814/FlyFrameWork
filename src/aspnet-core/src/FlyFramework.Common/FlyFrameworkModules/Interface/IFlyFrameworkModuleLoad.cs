using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleLoad
    {
        List<IFlyFrameworkBaseModuleDescritor> GetModuleDescritors(IServiceCollection service, Type startupModuleType);
    }
}