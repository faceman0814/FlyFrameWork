namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkModuleContainer
    {
        IReadOnlyList<IFlyFrameworkBaseModuleDescritor> Modules { get; }
    }
}