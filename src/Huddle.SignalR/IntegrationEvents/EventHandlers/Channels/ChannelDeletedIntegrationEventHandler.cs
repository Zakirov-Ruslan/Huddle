using Huddle.EventBus.Abstractions;
using Huddle.SignalR.IntegrationEvents.Events.Channels;
using Microsoft.AspNetCore.SignalR;
using Ordering.SignalrHub;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Channels
{
    public class ChannelDeletedIntegrationEventHandler : IIntegrationEventHandler<ChannelDeletedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<ChannelDeletedIntegrationEvent> _logger;

        public ChannelDeletedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext, ILogger<ChannelDeletedIntegrationEvent> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(ChannelDeletedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var deletedChannel = new { @event.ChannelId, @event.ServerId };

            await _hubContext.Clients.Group($"server:{@event.ServerId}").SendAsync("DeleteChannel", deletedChannel);
        }
    }
}
