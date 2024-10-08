// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FlyFramework.Dependencys;

using System;

namespace FlyFramework.LazyModule.LazyDefinition
{
    public interface IFlyFrameworkLazy
    {
        Lazy<T> LazyGetRequiredService<T>();

        Lazy<object> LazyGetRequiredService(Type serviceType);

        Lazy<T?> LazyGetService<T>() where T : class;

        Lazy<object?> LazyGetService(Type serviceType);

        Lazy<T> LazyGetService<T>(T defaultValue);

        Lazy<object> LazyGetService(Type serviceType, object defaultValue);

        Lazy<object> LazyGetService(Type serviceType, Func<IServiceProvider, object> factory);

        Lazy<T> LazyGetService<T>(Func<IServiceProvider, object> factory);
    }
}