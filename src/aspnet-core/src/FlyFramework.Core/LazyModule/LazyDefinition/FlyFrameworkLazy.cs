// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FlyFramework.Dependencys;
using FlyFramework.Extentions;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Concurrent;
namespace FlyFramework.LazyModule.LazyDefinition
{
    public class FlyFrameworkLazy : IFlyFrameworkLazy, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public FlyFrameworkLazy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Lazy<T> LazyGetRequiredService<T>()
        {
            return new Lazy<T>(() => _serviceProvider.GetRequiredService<T>());
        }

        public Lazy<object> LazyGetRequiredService(Type serviceType)
        {
            return new Lazy<object>(() => _serviceProvider.GetRequiredService(serviceType));
        }

        public Lazy<T?> LazyGetService<T>() where T : class
        {
            return new Lazy<T?>(() => _serviceProvider.GetService<T>());
        }

        public Lazy<object?> LazyGetService(Type serviceType)
        {
            return new Lazy<object?>(() => _serviceProvider.GetService(serviceType));
        }

        public Lazy<T> LazyGetService<T>(T defaultValue)
        {
            return new Lazy<T>(() => _serviceProvider.GetService<T>() ?? defaultValue);
        }

        public Lazy<object> LazyGetService(Type serviceType, object defaultValue)
        {
            return new Lazy<object>(() => _serviceProvider.GetService(serviceType) ?? defaultValue);
        }

        public Lazy<object> LazyGetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
            return new Lazy<object>(() => _serviceProvider.GetService(serviceType) ?? factory(_serviceProvider));
        }

        public Lazy<T> LazyGetService<T>(Func<IServiceProvider, object> factory)
        {
            return new Lazy<T>(() => (T)(_serviceProvider.GetService<T>() ?? factory(_serviceProvider)));
        }
    }
}