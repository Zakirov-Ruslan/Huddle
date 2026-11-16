using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Application.UnitTests;

internal static class Builders
{
    public static readonly Guid OwnerId = Guid.NewGuid();
    public static readonly Guid MemberId = Guid.NewGuid();

    public static Server Server(string name = "Huddle") => new(OwnerId, name, isPrivate: false);

    public static Member Member(Guid serverId, Guid? identityId = null) => new(serverId, identityId ?? MemberId);

    public static List<MessageAttachment> Attachments(int count)
    {
        var attachments = new List<MessageAttachment>();
        for (var index = 0; index < count; index++)
        {
            attachments.Add(new MessageAttachment(
                Guid.NewGuid(),
                $"file-{index}.png",
                "image/png",
                100 + index,
                isImage: true,
                index));
        }

        return attachments;
    }
}
