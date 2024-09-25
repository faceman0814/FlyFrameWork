namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IObjectAccessor<T>
    {
        T? Value { get; set; }
    }
}