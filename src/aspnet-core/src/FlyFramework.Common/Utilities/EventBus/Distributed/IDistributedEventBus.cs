using System.Threading;
using System.Threading.Tasks;
namespace FlyFramework.Utilities.EventBus.Distributed
{
    public interface IDistributedEventBus
    {
        Task PublishAsync<TEventData>(TEventData eventData, CancellationToken cancellationToken = default);
    }
}
