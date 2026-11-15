using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class RemoveServerAvatarCommandHandler : IRequestHandler<RemoveServerAvatarCommand, bool>
    {
        private readonly IServerRepository _serverRepository;

        public RemoveServerAvatarCommandHandler(IServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        public async Task<bool> Handle(RemoveServerAvatarCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");

            if (server.OwnerIdentityId != request.CommandSenderIdentityId)
                throw new ForbiddenAccessException("User are not owner of this server");

            server.RemoveAvatar();

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
