using Huddle.Channel.Application.Dto;

namespace Huddle.Channel.Application.Queries.Messages
{
    public interface IMessagesQueries
    {
        Task<IEnumerable<MessageDto>> GetRecentAsync(Guid chatId, int pageSize);
        Task<IEnumerable<MessageDto>> GetOlderAsync(Guid chatId, int pageSize, Guid beforeMessageId);
    }
}
