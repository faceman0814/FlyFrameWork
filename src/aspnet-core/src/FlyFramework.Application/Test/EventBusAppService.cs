using DotNetCore.CAP;

using FaceMan.DynamicWebAPI;

using FlyFramework.ApplicationServices;
using FlyFramework.Dependencys;
using FlyFramework.Utilities.EventBus;
using FlyFramework.Utilities.EventBus.Distributed;
using FlyFramework.Utilities.EventBus.Local;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;
namespace FlyFramework.Test
{
    /// <summary>
    /// EventBus应用服务
    /// </summary>
    public class EventBusAppService : ApplicationService, IApplicationService
    {
        private readonly ILocalEventBus _localEventBus;
        private readonly IDistributedEventBus _distributedEventBus;

        public EventBusAppService(IServiceProvider serviceProvider, ILocalEventBus localEventBus, IDistributedEventBus distributedEventBus)
        {
            _localEventBus = localEventBus;
            _distributedEventBus = distributedEventBus;
        }

        /// <summary>
        /// 本地事件总线发布测试事件
        /// </summary>
        /// <returns></returns>
        public async Task LocalPublish()
        {
            await _localEventBus.PublishAsync(new TestEventData { TestStr = "LocalEventBus" + Guid.NewGuid().ToString() });
        }

        /// <summary>
        /// 分布式事件总线发布测试事件
        /// </summary>
        /// <returns></returns>
        public async Task Distributed()
        {
            await _distributedEventBus.PublishAsync(new TestEventData { TestStr = "DistributedEventBus" + Guid.NewGuid().ToString() });
        }

        /// <summary>
        /// 检查接收到的消息
        /// </summary>
        /// <param name="test"></param>
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
