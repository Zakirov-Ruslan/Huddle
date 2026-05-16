using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain;

namespace Huddle.Channel.Application.Queries.Messages
{
    public interface IMessagesQueries
    {
        Task<PaginatedItems<MessageDto>> GetMessages(Guid userIdentity, Guid channelId, Guid? cursor = null, bool older = true, int limit = 50);
    }
}
