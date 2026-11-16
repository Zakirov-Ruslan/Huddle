using Huddle.Channel.Application.Commands.Invite;
using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Application.Validations;

namespace Huddle.Channel.Application.UnitTests.Validation;

public class AcceptInviteCommandValidatorTests
{
    private readonly AcceptInviteCommandValidator _validator = new();

    [Fact]
    public void Valid_WhenCodeHasExpectedLength()
    {
        var result = _validator.Validate(new AcceptInviteCommand("abcd123456", Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcd12345")]
    [InlineData("abcd1234567")]
    public void Invalid_WhenCodeHasUnexpectedLength(string code)
    {
        var result = _validator.Validate(new AcceptInviteCommand(code, Guid.NewGuid()));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AcceptInviteCommand.InviteCode));
    }
}

public class CreateMessageCommandValidatorTests
{
    private readonly CreateMessageCommandValidator _validator = new();

    private static CreateMessageCommand Command(string? text, params Guid[] attachmentIds) =>
        new(Guid.NewGuid(), Guid.NewGuid(), text!, attachmentIds);

    [Fact]
    public void Valid_WithTextOnly()
    {
        Assert.True(_validator.Validate(Command("hello")).IsValid);
    }

    [Fact]
    public void Valid_WithAttachmentsOnly()
    {
        Assert.True(_validator.Validate(Command("", Guid.NewGuid())).IsValid);
    }

    [Fact]
    public void Invalid_WithoutTextAndAttachments()
    {
        var result = _validator.Validate(Command(""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Message must contain text or at least one attachment");
    }

    [Fact]
    public void Invalid_WithNullTextAndAttachments()
    {
        Assert.False(_validator.Validate(Command(null)).IsValid);
    }

    [Fact]
    public void Valid_WithTextOfMaximumLength()
    {
        Assert.True(_validator.Validate(Command(new string('a', 100))).IsValid);
    }

    [Fact]
    public void Invalid_WithTextLongerThanMaximumLength()
    {
        var result = _validator.Validate(Command(new string('a', 101)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateMessageCommand.Text));
    }
}
