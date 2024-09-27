using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.EventBus.Distributed
{
    public interface IDistributedEventHandler<in TEventData> : IEventHandler
    {
        Task Handle(TEventData eventData, CancellationToken cancellationToken = default);
    }
}
