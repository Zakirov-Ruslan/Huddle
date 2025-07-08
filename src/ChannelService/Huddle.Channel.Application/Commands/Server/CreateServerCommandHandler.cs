using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Server
{
    public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, bool>
    {
        private readonly ILogger<CreateServerCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;

        public CreateServerCommandHandler(ILogger<CreateServerCommandHandler> logger, IServerRepository serverRepository)
        {
            _logger = logger;
            _serverRepository = serverRepository;
        }

        public async Task<bool> Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            Domain.Aggregates.ServerAggregate.Server server = new(request.CreatorId, request.Name, request.IsPrivate);

            _serverRepository.Add(server);
            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
