using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;

namespace Huddle.Channel.Application.Queries.Messages
{
    public class MessagesQueries : IMessagesQueries
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMapper _mapper;

        public MessagesQueries(IMessageRepository messageRepository, IMapper mapper)
        {
            this.messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> GetOlderAsync(Guid chatId, int pageSize, Guid beforeThan)
        {
            var messages = await messageRepository.GetOlderAsync(chatId, pageSize, beforeThan);

            return messages.Select(_mapper.Map<MessageDto>);
        }

        public async Task<IEnumerable<MessageDto>> GetRecentAsync(Guid chatId, int pageSize)
        {
            var messages = await messageRepository.GetRecentAsync(chatId, pageSize);

            return messages.Select(_mapper.Map<MessageDto>);
        }
    }
}
