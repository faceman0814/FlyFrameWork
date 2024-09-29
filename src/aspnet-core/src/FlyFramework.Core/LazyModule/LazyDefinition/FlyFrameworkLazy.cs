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
    public class FlyFrameworkLazy : IFlyFrameworkLazy, IServiceProvider
    {
        protected IServiceProvider ServiceProvider { get; }

        public FlyFrameworkLazy(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            CachedServices = new ConcurrentDictionary<ServiceIdentifier, Lazy<object?>>();
            CachedServices.TryAdd(new ServiceIdentifier(typeof(IServiceProvider)), new Lazy<object?>(() => ServiceProvider));
        }
        public virtual T LazyGetRequiredService<T>()
        {
            return (T)LazyGetRequiredService(typeof(T));
        }

        public virtual object LazyGetRequiredService(Type serviceType)
        {
            return this.GetRequiredService(serviceType);
        }

        public virtual T? LazyGetService<T>()
        {
            return (T?)LazyGetService(typeof(T));
        }

        public virtual object? LazyGetService(Type serviceType)
        {
            return GetService(serviceType);
        }

        public virtual T LazyGetService<T>(T defaultValue)
        {
            return GetService(defaultValue);
        }

        public virtual object LazyGetService(Type serviceType, object defaultValue)
        {
            return GetService(serviceType, defaultValue);
        }

        public virtual T LazyGetService<T>(Func<IServiceProvider, object> factory)
        {
            return GetService<T>(factory);
        }

        public virtual object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
            return GetService(serviceType, factory);
        }
        protected ConcurrentDictionary<ServiceIdentifier, Lazy<object?>> CachedServices { get; }
        public virtual object? GetService(Type serviceType)
        {
            return CachedServices.GetOrAdd(
                new ServiceIdentifier(serviceType),
                _ => new Lazy<object?>(() => ServiceProvider.GetService(serviceType))
            ).Value;
        }
        public T GetService<T>(T defaultValue)
        {
            return (T)GetService(typeof(T), defaultValue!);
        }

        public object GetService(Type serviceType, object defaultValue)
        {
            return GetService(serviceType) ?? defaultValue;
        }

        public T GetService<T>(Func<IServiceProvider, object> factory)
        {
            return (T)GetService(typeof(T), factory);
        }

        public object GetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
            return CachedServices.GetOrAdd(
                new ServiceIdentifier(serviceType),
                _ => new Lazy<object?>(() => factory(ServiceProvider))
            ).Value!;
        }

        public object? GetKeyedService(Type serviceType, object? serviceKey)
        {
            return CachedServices.GetOrAdd(
                new ServiceIdentifier(serviceKey, serviceType),
                _ => new Lazy<object?>(() => ServiceProvider.GetKeyedService(serviceType, serviceKey))
            ).Value;
        }

        public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
        {
            return CachedServices.GetOrAdd(
                new ServiceIdentifier(serviceKey, serviceType),
                _ => new Lazy<object?>(() => ServiceProvider.GetRequiredKeyedService(serviceType, serviceKey))
            ).Value!;
        }
    }
    public static class ServiceProviderKeyedServiceExtensions
    {
        public static object? GetKeyedService(this IServiceProvider provider, Type serviceType, object? serviceKey)
        {
            Check.NotNull(provider, nameof(provider));

            if (provider is IKeyedServiceProvider keyedServiceProvider)
            {
                return keyedServiceProvider.GetKeyedService(serviceType, serviceKey);
            }

            throw new InvalidOperationException("This service provider doesn't support keyed services.");
        }
    }
}
