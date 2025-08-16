using Huddle.SignalR.IntegrationEvents.EventHandlers.Channels;
using Huddle.SignalR.IntegrationEvents.EventHandlers.Messages;
using Huddle.SignalR.IntegrationEvents.Events.Channels;
using Huddle.SignalR.IntegrationEvents.Events.Messages;

namespace Huddle.SignalR.Extensions
{
    public static class DependencyInjection
    {
        public static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        {
            eventBus.AddSubscription<ChannelCreatedIntegrationEvent, ChannelCreatedIntegrationEventHandler>();
            eventBus.AddSubscription<ChannelDeletedIntegrationEvent, ChannelDeletedIntegrationEventHandler>();

            eventBus.AddSubscription<MessageCreatedIntegrationEvent, MessageCreatedIntegrationEventHandler>();
            eventBus.AddSubscription<MessageDeletedIntegrationEvent, MessageDeletedIntegrationEventHandler>();
        }
    }
}
