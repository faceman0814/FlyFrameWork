using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.EventBus.Distributed
{
    public interface IDistributedEventBus
    {
        Task PublishAsync<TEventData>(TEventData eventData, CancellationToken cancellationToken = default);
    }
}
