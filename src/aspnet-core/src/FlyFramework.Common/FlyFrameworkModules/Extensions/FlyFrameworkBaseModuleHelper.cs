using FlyFramework.Common.FlyFrameworkModules.Interface;
using FlyFramework.Common.FlyFrameworkModules.Modules;
using FlyFramework.Common.Reflection;

using System.Reflection;


namespace FlyFramework.Common.FlyFrameworkModules.Extensions
{
    internal static class FlyFrameworkBaseModuleHelper
    {
        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<Type> LoadModules(Type moduleType)
        {
            FlyFrameworkBaseModule.CheckModuleType(moduleType);
            var moduleTypes = new List<Type>();
            GetDependsAllModuleType(moduleType, moduleTypes);
            return moduleTypes;
        }

        //public static Assembly[] GetAllAssemblies(Type moduleType)
        //{
        //    var assemblies = new List<Assembly>();

        //    var additionalAssemblyDescriptors = moduleType
        //        .GetCustomAttributes()
        //        .OfType<IAdditionalModuleAssemblyProvider>();

        //    foreach (var descriptor in additionalAssemblyDescriptors)
        //    {
        //        foreach (var assembly in descriptor.GetAssemblies())
        //        {
        //            assemblies.AddIfNotContains(assembly);
        //        }
        //    }

        //    assemblies.Add(moduleType.Assembly);

        //    return assemblies.ToArray();
        //}

        public static void GetDependsAllModuleType(Type moduleType, List<Type> moduleTypes)
        {
            FlyFrameworkBaseModule.CheckModuleType(moduleType);
            if (moduleTypes.Contains(moduleType))
            {
                return;
            }

            moduleTypes.Add(moduleType);

            foreach (var dependModule in DependModuleTypes(moduleType))
            {
                GetDependsAllModuleType(dependModule, moduleTypes);
            }
        }

        public static List<Type> DependModuleTypes(Type moduleType)
        {
            FlyFrameworkBaseModule.CheckModuleType(moduleType);

            var dependencies = new List<Type>();

            var dependencyDescriptors = moduleType
                .GetCustomAttributes()
                .OfType<IDependsAttrProvider>();

            foreach (var descriptor in dependencyDescriptors)
            {
                foreach (var dependedModuleType in descriptor.GetDependsModulesType())
                {
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }

            return dependencies;
        }
    }
}