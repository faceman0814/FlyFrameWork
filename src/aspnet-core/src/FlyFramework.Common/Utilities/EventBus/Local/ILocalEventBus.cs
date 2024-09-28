using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework.Utilities.EventBus.Local
{
    public interface ILocalEventBus
    {
        Task PublishAsync<TEventData>(TEventData eventData, CancellationToken cancellationToken = default);
    }
}
