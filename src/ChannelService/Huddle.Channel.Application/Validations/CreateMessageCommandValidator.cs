using FluentValidation;
using Huddle.Channel.Application.Commands.Message;

namespace Huddle.Channel.Application.Validations
{
    public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
    {
        public CreateMessageCommandValidator()
        {
            RuleFor(command => command.Text).NotEmpty().MaximumLength(100);
        }
    }
}
