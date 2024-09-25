using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IModuleLoad
    {
        List<IBaseModuleDescritor> GetModuleDescritors(IServiceCollection service, Type startupModuleType);
    }
}