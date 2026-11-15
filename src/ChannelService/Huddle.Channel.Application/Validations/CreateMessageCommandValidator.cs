using FluentValidation;
using Huddle.Channel.Application.Commands.Message;

namespace Huddle.Channel.Application.Validations
{
    public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
    {
        public CreateMessageCommandValidator()
        {
            RuleFor(command => command.Text).MaximumLength(100);

            RuleFor(command => command)
                .Must(command => !string.IsNullOrWhiteSpace(command.Text) || command.AttachmentIds.Count > 0)
                .WithMessage("Message must contain text or at least one attachment");
        }
    }
}
