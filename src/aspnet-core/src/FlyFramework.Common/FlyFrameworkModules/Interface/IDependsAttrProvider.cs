using System;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IDependsAttrProvider
    {
        Type[] GetDependsModulesType();
    }
}