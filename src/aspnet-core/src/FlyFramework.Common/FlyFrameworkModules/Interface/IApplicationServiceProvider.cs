using System;
using System.Threading.Tasks;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IApplicationServiceProvider
    {
        void Initialize(IServiceProvider serviceProvider);

        Task InitializeAsync(IServiceProvider serviceProvider);
    }
}