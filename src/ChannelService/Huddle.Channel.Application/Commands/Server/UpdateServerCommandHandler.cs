using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Server
{
    public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, bool>
    {
        private readonly ILogger<UpdateServerCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;
        private readonly IFilesClient _filesClient;

        public UpdateServerCommandHandler(
            ILogger<UpdateServerCommandHandler> logger,
            IServerRepository serverRepository,
            IFilesClient filesClient)
        {
            _logger = logger;
            _serverRepository = serverRepository;
            _filesClient = filesClient;
        }

        public async Task<bool> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.Id)
                ?? throw new KeyNotFoundException("Server not found");

            if (server.OwnerIdentityId != request.CommandSenderIdenityId)
                throw new ForbiddenAccessException("User are not owner of this server");

            if (request.RemoveAvatar)
            {
                server.RemoveAvatar();
            }
            else if (request.AvatarFileId.HasValue)
            {
                await _filesClient.ClaimAvatarAsync(request.CommandSenderIdenityId, request.AvatarFileId.Value, cancellationToken);

                server.SetAvatar(request.AvatarFileId.Value);
            }

            server.UpdateName(request.Name);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
