using Huddle.EventBus.Abstractions;
using Huddle.SignalR.IntegrationEvents.EventHandlers.Messages;
using Huddle.SignalR.IntegrationEvents.Events.Voice;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Voice
{
    public class VoiceParticipantLeftIntegrationEventHandler : IIntegrationEventHandler<VoiceParticipantLeft>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<MessageDeletedIntegrationEventHandler> _logger;

        public VoiceParticipantLeftIntegrationEventHandler(IHubContext<NotificationsHub> hubContext, ILogger<MessageDeletedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(VoiceParticipantLeft @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var participantLeave = new { @event.ServerId, @event.ChannelId, @event.UserId };

            await _hubContext.Clients.Group($"server:{@event.ServerId}").SendAsync("LeaveChannel", participantLeave);
        }
    }
}
