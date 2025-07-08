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
            await _serverRepository.Delete(request.ServerId);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
