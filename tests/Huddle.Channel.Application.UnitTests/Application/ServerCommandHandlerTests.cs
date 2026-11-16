using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Mapping;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Huddle.Channel.Domain.SeedWork;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class CreateServerCommandHandlerTests
{
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();
    private readonly IServerDtoFactory _dtoFactory = Substitute.For<IServerDtoFactory>();
    private readonly IFilesClient _filesClient = Substitute.For<IFilesClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ServerDto _expectedDto = new(Guid.NewGuid(), "Huddle", Builders.OwnerId, null, []);

    private Server? _addedServer;

    private CreateServerCommandHandler CreateHandler()
    {
        _serverRepository.UnitOfWork.Returns(_unitOfWork);
        _serverRepository.Add(Arg.Any<Server>()).Returns(call =>
        {
            _addedServer = call.Arg<Server>();
            return _addedServer;
        });
        _dtoFactory.Create(Arg.Any<Server>()).Returns(_expectedDto);

        return new CreateServerCommandHandler(
            NullLogger<CreateServerCommandHandler>.Instance,
            _serverRepository,
            _dtoFactory,
            _filesClient);
    }

    [Fact]
    public async Task Handle_WithoutAvatar_CreatesServerWithoutCallingFilesService()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(new CreateServerCommand(Builders.OwnerId, "Huddle", isPrivate: false), CancellationToken.None);

        Assert.Same(_expectedDto, result);
        Assert.NotNull(_addedServer);
        Assert.Equal("Huddle", _addedServer.Name);
        Assert.Equal(Builders.OwnerId, _addedServer.OwnerIdentityId);
        Assert.Null(_addedServer.AvatarFileId);
        await _filesClient.DidNotReceive()
            .ClaimAvatarAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithAvatar_ClaimsAvatarBeforeCreatingServer()
    {
        var handler = CreateHandler();
        var avatarFileId = Guid.NewGuid();

        await handler.Handle(
            new CreateServerCommand(Builders.OwnerId, "Huddle", isPrivate: false, avatarFileId),
            CancellationToken.None);

        await _filesClient.Received(1).ClaimAvatarAsync(Builders.OwnerId, avatarFileId, Arg.Any<CancellationToken>());
        Assert.NotNull(_addedServer);
        Assert.Equal(avatarFileId, _addedServer.AvatarFileId);
    }

    [Fact]
    public async Task Handle_WithInvalidName_ThrowsAndDoesNotPersist()
    {
        var handler = CreateHandler();

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(new CreateServerCommand(Builders.OwnerId, "", isPrivate: false), CancellationToken.None));

        _serverRepository.DidNotReceive().Add(Arg.Any<Server>());
    }
}

public class UpdateServerCommandHandlerTests
{
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();
    private readonly IFilesClient _filesClient = Substitute.For<IFilesClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private UpdateServerCommandHandler CreateHandler()
    {
        _serverRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        return new UpdateServerCommandHandler(
            NullLogger<UpdateServerCommandHandler>.Instance,
            _serverRepository,
            _filesClient);
    }

    [Fact]
    public async Task Handle_WithUnknownServer_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(Arg.Any<Guid>()).Returns((Server?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateServerCommand(Guid.NewGuid(), "Renamed", Builders.OwnerId, null, false),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSenderIsNotOwner_Throws()
    {
        var handler = CreateHandler();
        var server = Builders.Server();
        _serverRepository.GetAsync(server.Id).Returns(server);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(
            new UpdateServerCommand(server.Id, "Renamed", Guid.NewGuid(), null, false),
            CancellationToken.None));

        Assert.Equal("Huddle", server.Name);
    }

    [Fact]
    public async Task Handle_WithNewAvatar_ClaimsItAndUpdatesServer()
    {
        var handler = CreateHandler();
        var server = Builders.Server();
        var avatarFileId = Guid.NewGuid();
        _serverRepository.GetAsync(server.Id).Returns(server);

        var result = await handler.Handle(
            new UpdateServerCommand(server.Id, "Renamed", Builders.OwnerId, avatarFileId, false),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal("Renamed", server.Name);
        Assert.Equal(avatarFileId, server.AvatarFileId);
        await _filesClient.Received(1).ClaimAvatarAsync(Builders.OwnerId, avatarFileId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithRemoveAvatar_ClearsAvatarWithoutCallingFilesService()
    {
        var handler = CreateHandler();
        var server = Builders.Server();
        server.SetAvatar(Guid.NewGuid());
        _serverRepository.GetAsync(server.Id).Returns(server);

        await handler.Handle(
            new UpdateServerCommand(server.Id, "Renamed", Builders.OwnerId, null, removeAvatar: true),
            CancellationToken.None);

        Assert.Null(server.AvatarFileId);
        await _filesClient.DidNotReceive()
            .ClaimAvatarAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}

public class ServerAvatarCommandHandlerTests
{
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();
    private readonly IFilesClient _filesClient = Substitute.For<IFilesClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task UpdateAvatar_WithUnknownServer_Throws()
    {
        _serverRepository.GetAsync(Arg.Any<Guid>()).Returns((Server?)null);
        var handler = new UpdateServerAvatarCommandHandler(_serverRepository, _filesClient);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateServerAvatarCommand(Guid.NewGuid(), Guid.NewGuid(), Builders.OwnerId),
            CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAvatar_WhenSenderIsNotOwner_ThrowsWithoutClaimingFile()
    {
        var server = Builders.Server();
        _serverRepository.GetAsync(server.Id).Returns(server);
        var handler = new UpdateServerAvatarCommandHandler(_serverRepository, _filesClient);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(
            new UpdateServerAvatarCommand(server.Id, Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None));

        await _filesClient.DidNotReceive()
            .ClaimAvatarAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        Assert.Null(server.AvatarFileId);
    }

    [Fact]
    public async Task UpdateAvatar_WhenSenderIsOwner_ClaimsFileAndSavesServer()
    {
        _serverRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        var server = Builders.Server();
        var fileId = Guid.NewGuid();
        _serverRepository.GetAsync(server.Id).Returns(server);
        var handler = new UpdateServerAvatarCommandHandler(_serverRepository, _filesClient);

        var result = await handler.Handle(
            new UpdateServerAvatarCommand(server.Id, fileId, Builders.OwnerId),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(fileId, server.AvatarFileId);
        await _filesClient.Received(1).ClaimAvatarAsync(Builders.OwnerId, fileId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAvatar_WhenSenderIsOwner_ClearsAvatar()
    {
        _serverRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        var server = Builders.Server();
        server.SetAvatar(Guid.NewGuid());
        _serverRepository.GetAsync(server.Id).Returns(server);
        var handler = new RemoveServerAvatarCommandHandler(_serverRepository);

        var result = await handler.Handle(
            new RemoveServerAvatarCommand(server.Id, Builders.OwnerId),
            CancellationToken.None);

        Assert.True(result);
        Assert.Null(server.AvatarFileId);
    }

    [Fact]
    public async Task RemoveAvatar_WhenSenderIsNotOwner_Throws()
    {
        var server = Builders.Server();
        _serverRepository.GetAsync(server.Id).Returns(server);
        var handler = new RemoveServerAvatarCommandHandler(_serverRepository);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(
            new RemoveServerAvatarCommand(server.Id, Guid.NewGuid()),
            CancellationToken.None));
    }
}

public class DeleteServerCommandHandlerTests
{
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private DeleteServerCommandHandler CreateHandler()
    {
        _serverRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        return new DeleteServerCommandHandler(NullLogger<DeleteServerCommandHandler>.Instance, _serverRepository);
    }

    [Fact]
    public async Task Handle_WithUnknownServer_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(Arg.Any<Guid>()).Returns((Server?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new DeleteServerCommand(Guid.NewGuid(), Builders.OwnerId),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSenderIsNotOwner_ThrowsAndKeepsServer()
    {
        var handler = CreateHandler();
        var server = Builders.Server();
        _serverRepository.GetAsync(server.Id).Returns(server);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(
            new DeleteServerCommand(server.Id, Guid.NewGuid()),
            CancellationToken.None));

        await _serverRepository.DidNotReceive().Delete(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Handle_WhenSenderIsOwner_DeletesServer()
    {
        var handler = CreateHandler();
        var server = Builders.Server();
        _serverRepository.GetAsync(server.Id).Returns(server);

        var result = await handler.Handle(new DeleteServerCommand(server.Id, Builders.OwnerId), CancellationToken.None);

        Assert.True(result);
        await _serverRepository.Received(1).Delete(server.Id);
    }
}
