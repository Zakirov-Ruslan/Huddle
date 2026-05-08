using Huddle.Channel.Application.Dto;
using Huddle.EventBus.Abstractions;
using Huddle.SignalR.IntegrationEvents.Events.Channels;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Channels
{
    public class ChannelCreatedIntegrationEventHandler : IIntegrationEventHandler<ChannelCreatedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<ChannelCreatedIntegrationEventHandler> _logger;

        public ChannelCreatedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext, ILogger<ChannelCreatedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(ChannelCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var createdChannel = new ChannelDto(@event.Id, @event.ServerId, @event.Name, @event.Type);
            Notification<ChannelDto> notification = new(createdChannel, @event.InitiatorSessionId);

            await _hubContext.Clients.Group($"server:{@event.ServerId}").SendAsync("CreateChannel", notification);
        }
    }
}
