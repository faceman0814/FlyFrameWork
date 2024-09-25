namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IBaseModule : IPreConfigureServices
    {
        void ConfigerService(ServiceConfigerContext context);

        void InitApplication(InitApplicationContext context);

        void LaterInitApplication(InitApplicationContext context);
    }
}