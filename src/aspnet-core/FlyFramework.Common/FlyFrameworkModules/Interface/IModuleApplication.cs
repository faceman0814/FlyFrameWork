using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IModuleApplication : IModuleContainer
    {
        Type StartModuleType { get; }
        IServiceCollection Services { get; }

        IServiceProvider ServiceProvider { get; }

        void ConfigerService();

        void InitApplication(IServiceProvider serviceProvider);

        Task InitApplicationAsync(IServiceProvider serviceProvider);
    }
}