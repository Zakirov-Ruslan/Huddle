using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public  class UpdateServerCommand : IRequest<bool>
    {
        public Guid CommandSenderIdenityId { get; private set; }
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public UpdateServerCommand(Guid id, string name, Guid commandSenderIdenityId)
        {
            Id = id;
            Name = name;
            CommandSenderIdenityId = commandSenderIdenityId;
        }
    }
}
