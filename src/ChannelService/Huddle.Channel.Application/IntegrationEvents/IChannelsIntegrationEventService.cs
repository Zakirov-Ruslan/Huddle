using Huddle.EventBus.Events;

namespace Huddle.Channel.Application.IntegrationEvents
{
    public interface IChannelsIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}
