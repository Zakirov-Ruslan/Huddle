using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public  class UpdateServerCommand : IRequest<bool>
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public UpdateServerCommand(Guid Id, string name)
        {
            this.Id = Id;
            Name = name;
        }
    }
}
