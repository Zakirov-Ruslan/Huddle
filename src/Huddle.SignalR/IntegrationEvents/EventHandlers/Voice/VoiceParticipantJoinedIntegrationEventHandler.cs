using Huddle.EventBus.Abstractions;
using Huddle.SignalR.IntegrationEvents.Events.Voice;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Voice
{
    /// <summary>
    /// Forwards a join to everyone who has the server open. The payload carries the same data as the
    /// snapshot endpoint plus the state version, so the client can order the push against its snapshot.
    /// </summary>
    public class VoiceParticipantJoinedIntegrationEventHandler : IIntegrationEventHandler<VoiceParticipantJoined>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<VoiceParticipantJoinedIntegrationEventHandler> _logger;

        public VoiceParticipantJoinedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<VoiceParticipantJoinedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(VoiceParticipantJoined @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var participant = new
            {
                @event.ServerId,
                @event.ChannelId,
                @event.UserId,
                @event.DisplayName,
                @event.JoinedAt,
                @event.Version
            };

            await _hubContext.Clients.Group($"server:{@event.ServerId}").SendAsync("VoiceParticipantJoined", participant);
        }
    }
}
