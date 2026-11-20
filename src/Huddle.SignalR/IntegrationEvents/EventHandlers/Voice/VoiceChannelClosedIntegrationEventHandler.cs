using Huddle.EventBus.Abstractions;
using Huddle.SignalR.IntegrationEvents.Events.Voice;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Voice
{
    public class VoiceChannelClosedIntegrationEventHandler : IIntegrationEventHandler<VoiceChannelClosed>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<VoiceChannelClosedIntegrationEventHandler> _logger;

        public VoiceChannelClosedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<VoiceChannelClosedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(VoiceChannelClosed @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var channel = new { @event.ServerId, @event.ChannelId, @event.Version };

            await _hubContext.Clients.Group($"server:{@event.ServerId}").SendAsync("VoiceChannelClosed", channel);
        }
    }
}
