using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.Commands.Server
{
    public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, ServerDto>
    {
        private readonly ILogger<CreateServerCommandHandler> _logger;
        private readonly IServerRepository _serverRepository;
        private readonly IMapper _mapper;

        public CreateServerCommandHandler(ILogger<CreateServerCommandHandler> logger, IServerRepository serverRepository, IMapper mapper)
        {
            _logger = logger;
            _serverRepository = serverRepository;
            _mapper = mapper;
        }

        public async Task<ServerDto> Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            Domain.Aggregates.ServerAggregate.Server server = new(request.CreatorId, request.Name, request.IsPrivate);

            _serverRepository.Add(server);
            
            await _serverRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return _mapper.Map<ServerDto>(server);
        }
    }
}
