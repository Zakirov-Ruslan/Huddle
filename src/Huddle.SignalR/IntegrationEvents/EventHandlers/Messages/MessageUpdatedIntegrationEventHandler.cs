using Huddle.Channel.Application.IntegrationEvents.Events.Messages;
using Huddle.EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Ordering.SignalrHub;
using System.Reflection;

namespace Huddle.SignalR.IntegrationEvents.EventHandlers.Messages
{
    public class MessageUpdatedIntegrationEventHandler : IIntegrationEventHandler<MessageUpdatedIntegretaionEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<MessageUpdatedIntegrationEventHandler> _logger;

        public MessageUpdatedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext, ILogger<MessageUpdatedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(MessageUpdatedIntegretaionEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Assembly.GetExecutingAssembly().FullName, @event);

            var messageUpdate = new { @event.ChannelId, @event.MessageId, @event.Text };

            await _hubContext.Clients.Group($"channel:{@event.ChannelId}").SendAsync("UpdateMessage", messageUpdate);
        }
    }
}
