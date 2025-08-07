using Huddle.Channel.Application.IntegrationEvents.Events.Messages;
using Huddle.EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Ordering.SignalrHub;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Messages
{
    public class MessageDeletedIntegrationEventHandler : IIntegrationEventHandler<MessageDeletedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<MessageDeletedIntegrationEventHandler> _logger;

        public MessageDeletedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext, ILogger<MessageDeletedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(MessageDeletedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var deletedMessage = new { @event.ChannelId, @event.MessageId };

            await _hubContext.Clients.Group($"channel:{@event.ChannelId}").SendAsync("DeleteMessage", deletedMessage);
        }
    }
}
