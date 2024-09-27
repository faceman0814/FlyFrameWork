using FlyFramework.Common.FlyFrameworkModules.Interface;

using System;
using System.Reflection;
namespace FlyFramework.Common.FlyFrameworkModules.Modules
{
    public class FlyFrameworkBaseModuleDescritor : IFlyFrameworkBaseModuleDescritor
    {
        public Type ModuleType { get; }

        public IFlyFrameworkBaseModule Instance { get; }

        public FlyFrameworkBaseModuleDescritor(Type type, IFlyFrameworkBaseModule instance)
        {
            if (!type.GetTypeInfo().IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException($"Given module instance ({instance.GetType().AssemblyQualifiedName}) is not an instance of given module type: {type.AssemblyQualifiedName}");
            }
            ModuleType = type;
            Instance = instance;
        }
    }
}