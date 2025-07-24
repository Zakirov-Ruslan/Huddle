using Huddle.Channel.Application.Dto;

namespace Huddle.Channel.Application.Queries.Messages
{
    public interface IMessagesQueries
    {
        Task<IEnumerable<MessageDto>> GetRecentAsync(Guid chatId, int pageSize, Guid userIdentity);
        Task<IEnumerable<MessageDto>> GetOlderAsync(Guid channelId, int pageSize, Guid beforeThan, Guid userIdentity);
    }
}
