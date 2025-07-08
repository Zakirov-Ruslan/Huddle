using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand, bool>
    {
        private readonly ILogger<UpdateChannelCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;

        public UpdateChannelCommandHandler(ILogger<UpdateChannelCommandHandler> logger, IServerRepository serverRepository)
        {
            _logger = logger;
            _serverRepository = serverRepository;
        }

        public async Task<bool> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");

            var channel = server.Channels.FirstOrDefault(c => c.Id == request.Id)
                ?? throw new KeyNotFoundException("Channel not found");

            channel.ChangeName(request.Name);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
