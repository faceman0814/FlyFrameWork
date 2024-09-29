using FlyFramework.FlyFrameworkModules.Interface;

namespace FlyFramework.FlyFrameworkModules.Modules
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