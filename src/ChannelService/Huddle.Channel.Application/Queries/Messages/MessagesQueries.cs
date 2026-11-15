using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Mapping;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;

namespace Huddle.Channel.Application.Queries.Messages
{
    public class MessagesQueries : IMessagesQueries
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageDtoFactory _messageDtoFactory;
        private readonly IAccessService _accessService;

        public MessagesQueries(IMessageRepository messageRepository, IMessageDtoFactory messageDtoFactory, IAccessService accessService)
        {
            _messageRepository = messageRepository;
            _messageDtoFactory = messageDtoFactory;
            _accessService = accessService;
        }

        public async Task<PaginatedItems<Dto.MessageDto>> GetMessages(Guid userIdentityId, Guid channelId, Guid? cursor = null, bool older = true, int limit = 50)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(channelId, userIdentityId);
            if (!hasAccess)
                throw new ForbiddenAccessException("User dont have access to this channel");

            var paginatedMessages = await _messageRepository.GetMessagesAsync(channelId, cursor, older);

            var messagesDto = paginatedMessages.Items.Select(_messageDtoFactory.Create).ToList();

            return new PaginatedItems<Dto.MessageDto>(
                messagesDto,
                paginatedMessages.HasPrev,
                paginatedMessages.HasNext,
                paginatedMessages.NextCursor,
                paginatedMessages.PrevCursor
            );
        }
    }
}
