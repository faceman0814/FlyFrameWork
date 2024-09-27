using System;
namespace FlyFramework.Common.FlyFrameworkModules.Interface
{
    public interface IDependsAttrProvider
    {
        Type[] GetDependsModulesType();
    }
}