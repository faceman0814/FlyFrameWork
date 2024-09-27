using FlyFramework.Common.FlyFrameworkModules.Interface;

namespace FlyFramework.Common.FlyFrameworkModules.Modules
{
    public class ObjectAccessor<T> : IObjectAccessor<T>
    {
        public T? Value { get; set; }

        public ObjectAccessor()
        { }

        public ObjectAccessor(T value)
        {
            Value = value;
        }
    }
}