using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Mapping;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Server
{
    public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, ServerDto>
    {
        private readonly ILogger<CreateServerCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;
        private readonly IServerDtoFactory _serverDtoFactory;
        private readonly IFilesClient _filesClient;

        public CreateServerCommandHandler(
            ILogger<CreateServerCommandHandler> logger,
            IServerRepository serverRepository,
            IServerDtoFactory serverDtoFactory,
            IFilesClient filesClient)
        {
            _logger = logger;
            _serverRepository = serverRepository;
            _serverDtoFactory = serverDtoFactory;
            _filesClient = filesClient;
        }

        public async Task<ServerDto> Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            if (request.AvatarFileId.HasValue)
                await _filesClient.ClaimAvatarAsync(request.CreatorId, request.AvatarFileId.Value, cancellationToken);

            Domain.Aggregates.ServerAggregate.Server server = new(request.CreatorId, request.Name, request.IsPrivate, request.AvatarFileId);

            _serverRepository.Add(server);

            await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return _serverDtoFactory.Create(server);
        }
    }
}
