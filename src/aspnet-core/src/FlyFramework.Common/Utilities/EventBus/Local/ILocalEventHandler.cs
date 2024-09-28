using FlyFramework.Utilities.EventBus;

using MediatR;

using System.Threading;
using System.Threading.Tasks;
namespace FlyFramework.Utilities.EventBus.Local
{
    public interface ILocalEventHandler<in TEventData> : IEventHandler, INotificationHandler<TEventData> where TEventData : INotification
    {
        Task Handle(TEventData eventData, CancellationToken cancellationToken = default);
    }
}
