using Huddle.SignalR.IntegrationEvents.EventHandlers.Channels;
using Huddle.SignalR.IntegrationEvents.Events.Channels;

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
