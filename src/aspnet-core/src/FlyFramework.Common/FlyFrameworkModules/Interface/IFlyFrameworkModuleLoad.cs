using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleLoad
    {
        List<IFlyFrameworkBaseModuleDescritor> GetModuleDescritors(IServiceCollection service, Type startupModuleType);
    }
}