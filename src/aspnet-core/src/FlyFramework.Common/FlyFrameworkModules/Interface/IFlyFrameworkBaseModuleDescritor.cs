﻿using System;
namespace FlyFramework.FlyFrameworkModules.Interface
{
    public interface IFlyFrameworkBaseModuleDescritor
    {
        public Type ModuleType { get; }

        public IFlyFrameworkBaseModule Instance { get; }
    }
}