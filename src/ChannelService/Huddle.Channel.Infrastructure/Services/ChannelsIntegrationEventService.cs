using Huddle.Channel.Application.IntegrationEvents;
using Huddle.EventBus.Abstractions;
using Huddle.EventBus.Events;
using Huddle.IntegrationEventLogEF.Services;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Infrastructure.Services
{
    public class ChannelsIntegrationEventService(
        IEventBus eventBus,
        ChannelContext channelsContext,
        IIntegrationEventLogService integrationEventLogService,
        ILogger<ChannelsIntegrationEventService> logger) : IChannelsIntegrationEventService

    {
        private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        private readonly ChannelContext _channelsContext = channelsContext ?? throw new ArgumentNullException(nameof(channelsContext));
        private readonly IIntegrationEventLogService _eventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
        private readonly ILogger<ChannelsIntegrationEventService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", logEvt.EventId, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    await _eventBus.PublishAsync(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing integration event: {IntegrationEventId}", logEvt.EventId);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _channelsContext.GetCurrentTransaction());
        }
    }
}
