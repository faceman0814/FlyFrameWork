namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IModuleContainer
    {
        IReadOnlyList<IBaseModuleDescritor> Modules { get; }
    }
}