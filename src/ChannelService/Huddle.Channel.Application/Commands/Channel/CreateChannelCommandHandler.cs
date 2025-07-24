using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, ChannelDto>
    {
        private readonly ILogger<CreateChannelCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;
        private readonly IMapper _mapper;

        public CreateChannelCommandHandler(ILogger<CreateChannelCommandHandler> logger, IServerRepository serverRepository, IMapper mapper)
        {
            _logger = logger;
            _serverRepository = serverRepository;
            _mapper = mapper;
        }

        public async Task<ChannelDto> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
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

            await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return _mapper.Map<ChannelDto>(channel);
        }
    }
}
