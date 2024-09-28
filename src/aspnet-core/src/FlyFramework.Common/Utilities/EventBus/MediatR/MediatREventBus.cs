using FlyFramework.Dependencys;
using FlyFramework.Utilities.EventBus.Local;

using MediatR;

using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework.Utilities.EventBus.MediatR
{
    public class MediatREventBus : ILocalEventBus, ITransientDependency
    {
        private readonly IMediator _mediator;

        public MediatREventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task PublishAsync<TEventData>(TEventData eventData, CancellationToken cancellationToken)
        {
            return _mediator.Publish(eventData, cancellationToken);
        }
    }
}
