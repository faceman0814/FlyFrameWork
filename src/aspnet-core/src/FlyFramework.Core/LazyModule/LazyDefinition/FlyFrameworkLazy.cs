// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

using System;
namespace FlyFramework.LazyModule.LazyDefinition
{
    public class FlyFrameworkLazy<T> : IFlyFrameworkLazy<T>
    {
        private readonly Lazy<T> _value;

        public FlyFrameworkLazy(IServiceProvider serviceProvider)
        {
            _value = new Lazy<T>(() =>
            {
                return serviceProvider.GetService<T>();
            });
        }

        public T Value => _value.Value;
    }
}
