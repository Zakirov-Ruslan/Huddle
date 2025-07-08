using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public class CreateMemberCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public Guid IdentityId { get; private set; }

        public CreateMemberCommand(Guid serverId, Guid identityId)
        {
            ServerId = serverId;
            IdentityId = identityId;
        }
    }
}
