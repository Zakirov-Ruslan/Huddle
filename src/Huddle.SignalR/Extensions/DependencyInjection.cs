using Huddle.SignalR.IntegrationEvents.EventHandlers;
using Huddle.SignalR.IntegrationEvents.Events;

namespace Huddle.SignalR.Extensions
{
    public static class DependencyInjection
    {
        public static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        {
            eventBus.AddSubscription<ChannelCreatedIntegrationEvent, ChannelCreatedIntegrationEventHandler>();
        }
    }
}
