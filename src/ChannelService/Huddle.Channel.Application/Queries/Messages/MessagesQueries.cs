using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain;
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

        public async Task<PaginatedItems<MessageDto>> GetMessages(Guid userIdentityId, Guid channelId, Guid? cursor = null, int limit = 50)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(channelId, userIdentityId);
            if (!hasAccess)
                throw new ForbiddenAccessException("User dont have access to this channel");

            var paginatedMessages = await _messageRepository.GetMessagesAsync(channelId, cursor, limit);

            var messagesDto = paginatedMessages.Items.Select(_mapper.Map<MessageDto>);

            return new PaginatedItems<MessageDto>(
                messagesDto, 
                paginatedMessages.HasMore, 
                paginatedMessages.NextCursor
            );
        }
    }
}
