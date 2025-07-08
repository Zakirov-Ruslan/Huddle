using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class CreateServerCommand : IRequest<bool>
    {
        public Guid CreatorId { get; private set; }
        public string Name { get; private set; }
        public bool IsPrivate { get; private set; }

        public CreateServerCommand(Guid creatorId, string name, bool isPrivate)
        {
            CreatorId = creatorId;
            Name = name;
            IsPrivate = isPrivate;
        }
    }
}
