using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand, bool>
    {
        private readonly ILogger<DeleteChannelCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;

        public DeleteChannelCommandHandler(ILogger<DeleteChannelCommandHandler> logger, IServerRepository serverRepository)
        {
            _logger = logger;
            _serverRepository = serverRepository;
        }
        public async Task<bool> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");

            server.DeleteChannel(request.Id);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
