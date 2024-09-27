﻿using Microsoft.Extensions.DependencyInjection;

using System;

namespace FlyFramework.Common.Dependencys
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterLifeAttribute : Attribute
    {
        public virtual ServiceLifetime? Lifetime { get; set; }

        /// <summary>
        /// 添加一个服务，但是如果该服务已经存在，则不会抛出异常，而是直接返回 false
        /// </summary>
        public virtual bool TryRegister { get; set; }

        /// <summary>
        /// 替换
        /// </summary>
        public virtual bool ReplaceServices { get; set; }

        public RegisterLifeAttribute()
        {
        }

        public RegisterLifeAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}