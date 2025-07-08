using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, bool>
    {
        private readonly ILogger<CreateChannelCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;

        public CreateChannelCommandHandler(ILogger<CreateChannelCommandHandler> logger, IServerRepository serverRepository)
        {
            _logger = logger;
            _serverRepository = serverRepository;
        }

        public async Task<bool> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");

            var channelType = request.ChannelType.ToLower() switch
            {
                "text" => ChannelType.Text,
                "audio" => ChannelType.Audio,
                _ => throw new ArgumentException(nameof(request.ChannelType))
            };

            var channel = server.AddChannel(channelType, request.Name);

            return await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
