using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class UpdateServerAvatarCommandHandler : IRequestHandler<UpdateServerAvatarCommand, bool>
    {
        private readonly IServerRepository _serverRepository;
        private readonly IFilesClient _filesClient;

        public UpdateServerAvatarCommandHandler(IServerRepository serverRepository, IFilesClient filesClient)
        {
            _serverRepository = serverRepository;
            _filesClient = filesClient;
        }

        public async Task<bool> Handle(UpdateServerAvatarCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");

            if (server.OwnerIdentityId != request.CommandSenderIdentityId)
                throw new ForbiddenAccessException("User are not owner of this server");

            await _filesClient.ClaimAvatarAsync(request.CommandSenderIdentityId, request.AvatarFileId, cancellationToken);

            server.SetAvatar(request.AvatarFileId);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
