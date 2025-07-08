using Huddle.EventBus.Events;
using System.Threading.Tasks;

namespace Huddle.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
