namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IBaseModuleDescritor
    {
        public Type ModuleType { get; }

        public IBaseModule Instance { get; }
    }
}