using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public class DeleteMemberCommand : IRequest<bool>
    {
        public Guid MemberId { get; private set; }
        public Guid CommandSenderId { get; private set; }

        public DeleteMemberCommand(Guid memberId, Guid commandSenderId)
        {
            MemberId = memberId;
            CommandSenderId = commandSenderId;
        }
    }
}
