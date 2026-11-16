using Huddle.Channel.Application.Services;
using Huddle.EventBus.Abstractions;
using Huddle.EventBus.Events;

namespace Huddle.Channel.FunctionalTests;

/// <summary>
/// Integration events are still written to the outbox, but nothing is published to RabbitMQ during tests.
/// </summary>
internal sealed class NoopEventBus : IEventBus
{
    public Task PublishAsync(IntegrationEvent @event) => Task.CompletedTask;
}

/// <summary>Stands in for the file service: every requested file is accepted.</summary>
internal sealed class FakeFilesClient : IFilesClient
{
    public Task<IReadOnlyList<ClaimedFile>> ClaimAttachmentsAsync(
        Guid ownerId,
        IReadOnlyList<Guid> fileIds,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ClaimedFile> files = fileIds
            .Select(id => new ClaimedFile(id, "attachment.png", "image/png", 1024, IsImage: true))
            .ToList();

        return Task.FromResult(files);
    }

    public Task<ClaimedFile> ClaimAvatarAsync(Guid ownerId, Guid fileId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ClaimedFile(fileId, "avatar.png", "image/png", 2048, IsImage: true));
}
