// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Core.LazyModule.LazyDefinition
{
    public class FlyFrameworkLazy<T> : IFlyFrameworkLazy<T>
    {
        private readonly System.Lazy<T> _value;

        public FlyFrameworkLazy(IServiceProvider serviceProvider)
        {
            _value = new System.Lazy<T>(() =>
            {
                return serviceProvider.GetService<T>();
            });
        }

        public T Value => _value.Value;
    }
}
