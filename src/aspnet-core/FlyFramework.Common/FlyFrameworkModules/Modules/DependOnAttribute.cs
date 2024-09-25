using FlyFramework.Common.FlyFrameworkModules.Interface;

namespace FlyFramework.Common.FlyFrameworkModules.Modules
{
    public class DependOnAttribute : Attribute, IDependsAttrProvider
    {
        public Type[] DependedTypes { get; }

        public DependOnAttribute(params Type[] types)
        {
            DependedTypes = types ?? new Type[0];
        }

        public Type[] GetDependsModulesType()
        {
            return DependedTypes;
        }
    }
}