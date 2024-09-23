using DotNetCore.CAP;

using FlyFramework.Application.DynamicWebAPI;
using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Utilities.EventBus;
using FlyFramework.Common.Utilities.EventBus.Distributed;
using FlyFramework.Common.Utilities.EventBus.Local;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.Test
{
    public class EventBusAppService : ApplicationService, IApplicationService
    {
        private readonly ILocalEventBus _localEventBus;
        private readonly IDistributedEventBus _distributedEventBus;

        public EventBusAppService(ILocalEventBus localEventBus, IDistributedEventBus distributedEventBus)
        {
            _localEventBus = localEventBus;
            _distributedEventBus = distributedEventBus;
        }
        [HttpGet("Local")]
        public async Task LocalPublish()
        {
            await _localEventBus.PublishAsync(new TestEventData { TestStr = "LocalEventBus" + Guid.NewGuid().ToString() });
        }

        [HttpGet("Distributed")]
        public async Task Distributed()
        {
            await _distributedEventBus.PublishAsync(new TestEventData { TestStr = "DistributedEventBus" + Guid.NewGuid().ToString() });
        }

        [HttpGet]
        [CapSubscribe("Test")]
        public void CheckReceivedMessage(TestEventData test)
        {
            Console.WriteLine("~~~~~~~~" + test.TestStr);
        }
    }
    [EventName("Test")]
    public class TestEventData : INotification
    {
        public string TestStr { get; set; }
    }

    public class TestEventDataLocalEventHandler : ILocalEventHandler<TestEventData>
    {
        private readonly ILogger<TestEventDataLocalEventHandler> _logger;

        public TestEventDataLocalEventHandler(ILogger<TestEventDataLocalEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TestEventData eventData, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"TestEventDataLocalEventHandler: {eventData.TestStr}");
            return Task.CompletedTask;
        }
    }

    public class TestEventDataDistributedEventHandler : IDistributedEventHandler<TestEventData>, ITransientDependency
    {
        private readonly ILogger<TestEventDataDistributedEventHandler> _logger;

        public TestEventDataDistributedEventHandler(ILogger<TestEventDataDistributedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TestEventData eventData, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"TestEventDataDistributedEventHandler: {eventData.TestStr}");
            return Task.CompletedTask;
        }
    }
}
