using FluentValidation;
using Huddle.Channel.Application.Commands.Invite;

namespace Huddle.Channel.Application.Validations
{
    public class AcceptInviteCommandValidator : AbstractValidator<AcceptInviteCommand>
    {
        public AcceptInviteCommandValidator()
        {
            RuleFor(x => x.InviteCode).NotEmpty().Length(10);
        }
    }
}
