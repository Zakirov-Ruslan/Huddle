using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Server
{
    public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, bool>
    {
        private readonly ILogger<DeleteServerCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;

        public DeleteServerCommandHandler(ILogger<DeleteServerCommandHandler> logger, IServerRepository serverRepository)
        {
            _logger = logger;
            _serverRepository = serverRepository;
        }
        public async Task<bool> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");

            if (server.OwnerIdentityId != request.CommandSenderIdenityId)
                throw new ForbiddenAccessException("User are not owner of this server");

            await _serverRepository.Delete(request.ServerId);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
