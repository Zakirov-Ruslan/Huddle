using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;

namespace Huddle.Channel.Application.Queries.Messages
{
    public interface IMessagesQueries
    {
        Task<PaginatedItems<MessageDto>> GetMessages(Guid userIdentity, Guid channelId, Guid? cursor = null, int limit = 50);
    }
}
