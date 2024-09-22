﻿using DotNetCore.CAP;

using FlyFramework.Common.Dependencys;

using System.Reflection;

namespace FlyFramework.Common.Utilities.EventBus.Distributed.Cap
{
    public class CapDistributedEventBus : IDistributedEventBus, ITransientDependency
    {
        private readonly ICapPublisher _capBus;

        public CapDistributedEventBus(ICapPublisher capBus)
        {
            _capBus = capBus;
        }

        public Task PublishAsync<TEventData>(TEventData eventData, CancellationToken cancellationToken = default)
        {
            var sub = typeof(TEventData).GetCustomAttribute<EventNameAttribute>()?.Name;
            return _capBus.PublishAsync(sub ?? nameof(eventData), eventData, cancellationToken: cancellationToken);
        }
    }
}
