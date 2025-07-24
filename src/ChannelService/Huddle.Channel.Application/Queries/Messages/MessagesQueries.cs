using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;

namespace Huddle.Channel.Application.Queries.Messages
{
    public class MessagesQueries : IMessagesQueries
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IAccessService _accessService;

        public MessagesQueries(IMessageRepository messageRepository, IMapper mapper, IAccessService accessService)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _accessService = accessService;
        }

        public async Task<IEnumerable<MessageDto>> GetOlderAsync(Guid channelId, int pageSize, Guid beforeThan, Guid userIdentity)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(channelId, userIdentity);
            if (!hasAccess)
                throw new ForbiddenAccessException("User dont have access to this channel");

            var messages = await _messageRepository.GetOlderAsync(channelId, pageSize, beforeThan);

            return messages.Select(_mapper.Map<MessageDto>);
        }

        public async Task<IEnumerable<MessageDto>> GetRecentAsync(Guid channelId, int pageSize, Guid userIdentity)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(channelId, userIdentity);
            if (!hasAccess)
                throw new ForbiddenAccessException("User dont have access to this channel");

            var messages = await _messageRepository.GetRecentAsync(channelId, pageSize);

            return messages.Select(_mapper.Map<MessageDto>);
        }
    }
}
