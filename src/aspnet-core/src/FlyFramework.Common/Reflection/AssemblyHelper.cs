﻿using System;
using System.Collections.Generic;
using System.Reflection;
namespace FlyFramework.Reflection
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// 返回可以通过索引读取的只读数组
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetAllTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }
    }
}