using Huddle.Channel.Application.Dto;
using Huddle.EventBus.Abstractions;
using Huddle.SignalR.IntegrationEvents.Events.Messages;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Messages
{
    public class MessageCreatedIntegrationEventHandler : IIntegrationEventHandler<MessageCreatedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<MessageCreatedIntegrationEventHandler> _logger;

        public MessageCreatedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext, ILogger<MessageCreatedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(MessageCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            MessageDto createdMessage = new(@event.MessageId, @event.ChannelId, @event.AuthorId, @event.Text, @event.SentAt, false);

            Notification<MessageDto> notification = new(createdMessage, @event.InitiatorSessionId);

            await _hubContext.Clients.Group($"channel:{@event.ChannelId}").SendAsync("CreateMessage", notification);
        }
    }
}
