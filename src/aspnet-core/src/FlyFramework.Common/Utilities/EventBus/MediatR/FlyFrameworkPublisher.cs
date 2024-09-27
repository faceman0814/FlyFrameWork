using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.EventBus.MediatR
{
    public class FlyFrameworkPublisher : INotificationPublisher
    {
        public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                foreach (var handler in handlerExecutors)
                {
                    await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
                }
            }, cancellationToken);
        }
    }
}
