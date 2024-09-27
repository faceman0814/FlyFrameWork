using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleApplication : IFlyFrameworkModuleContainer
    {
        Type StartModuleType { get; }
        IServiceCollection Services { get; }

        IServiceProvider ServiceProvider { get; }

        void Initialize();

        void InitApplication(IServiceProvider serviceProvider);

        Task InitApplicationAsync(IServiceProvider serviceProvider);
    }
}