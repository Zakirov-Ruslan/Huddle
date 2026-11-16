using System.Net;
using System.Net.Http.Json;
using Huddle.Channel.Application.IntegrationEvents.Events.Messages;
using Huddle.Channel.Infrastructure;
using Huddle.IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;

namespace Huddle.Channel.FunctionalTests;

[Collection(ChannelApiCollection.Name)]
public class ChannelApiTests(ChannelApiFixture fixture) : ChannelApiTestBase(fixture)
{
    private readonly Guid _ownerId = Guid.NewGuid();

    [Fact]
    public async Task CreateServer_ReturnsCreatedServerWithDefaultChannels()
    {
        var client = CreateClient(_ownerId);
        var name = NewServerName();

        var response = await client.PostAsJsonAsync("/api/servers", new { name, isPrivate = false });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var server = await response.Content.ReadFromJsonAsync<ServerDto>();
        Assert.NotNull(server);
        Assert.Equal(name, server.Name);
        Assert.Equal(_ownerId, server.OwnerIdentityId);
        Assert.Equal(2, server.Channels.Count);
        Assert.Contains(server.Channels, channel => channel.Name == "Main" && channel.ChannelType == "Text");
        Assert.Contains(server.Channels, channel => channel.Name == "Main" && channel.ChannelType == "Voice");
        Assert.All(server.Channels, channel => Assert.NotEqual(Guid.Empty, channel.Id));
        Assert.EndsWith($"/api/Servers/{server.Id}", response.Headers.Location!.AbsolutePath);
    }

    [Fact]
    public async Task GetServer_ReturnsServerWithChannels()
    {
        var client = CreateClient(_ownerId);
        var created = await CreateServerAsync(client);

        var server = await client.GetFromJsonAsync<ServerDto>($"/api/servers/{created.Id}");

        Assert.NotNull(server);
        Assert.Equal(created.Id, server.Id);
        Assert.Equal(created.Name, server.Name);
        Assert.Equal(2, server.Channels.Count);
    }

    [Fact]
    public async Task GetMyServers_ContainsTheServerBecauseTheCreatorBecameAMember()
    {
        var client = CreateClient(_ownerId);
        var created = await CreateServerAsync(client);

        var servers = await client.GetFromJsonAsync<List<ServerDto>>("/api/servers/my");

        Assert.NotNull(servers);
        Assert.Contains(servers, server => server.Id == created.Id);
    }

    [Fact]
    public async Task GetServer_ByNonMember_ReturnsForbidden()
    {
        var created = await CreateServerAsync(CreateClient(_ownerId));

        var response = await CreateClient(Guid.NewGuid()).GetAsync($"/api/servers/{created.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ServerEndpoints_WithoutAuthenticatedUser_ReturnUnauthorized()
    {
        var response = await Fixture.CreateClient().GetAsync("/api/servers/my");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateInvite_ByServerOwner_ReturnsCodeOfFixedLength()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);

        var code = await CreateInviteAsync(client, server.Id);

        Assert.Equal(10, code.Length);
    }

    [Fact]
    public async Task CreateInvite_ByNonOwner_DoesNotCreateInvite()
    {
        var ownerClient = CreateClient(_ownerId);
        var server = await CreateServerAsync(ownerClient);

        var response = await CreateClient(Guid.NewGuid()).PostAsync($"/api/servers/{server.Id}/invites", null);

        Assert.False(response.IsSuccessStatusCode);

        var invites = await ownerClient.GetFromJsonAsync<List<InviteDto>>($"/api/servers/{server.Id}/invites");
        Assert.NotNull(invites);
        Assert.Empty(invites);
    }

    [Fact]
    public async Task AcceptInvite_AddsMemberAndExposesServerToTheNewMember()
    {
        var ownerClient = CreateClient(_ownerId);
        var server = await CreateServerAsync(ownerClient);
        var code = await CreateInviteAsync(ownerClient, server.Id);
        var inviteeId = Guid.NewGuid();
        var inviteeClient = CreateClient(inviteeId);

        var response = await AcceptInviteAsync(inviteeClient, code, Guid.NewGuid());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var servers = await inviteeClient.GetFromJsonAsync<List<ServerDto>>("/api/servers/my");
        Assert.NotNull(servers);
        Assert.Contains(servers, item => item.Id == server.Id);

        var members = await ownerClient.GetFromJsonAsync<PaginatedItems<MemberDto>>($"/api/servers/{server.Id}/members");
        Assert.NotNull(members);
        Assert.Contains(members.Items, member => member.IdentityId == inviteeId);
    }

    [Fact]
    public async Task AcceptInvite_WithNewRequestId_DoesNotDuplicateMember()
    {
        var ownerClient = CreateClient(_ownerId);
        var server = await CreateServerAsync(ownerClient);
        var code = await CreateInviteAsync(ownerClient, server.Id);
        var inviteeClient = CreateClient(Guid.NewGuid());

        await AcceptInviteAsync(inviteeClient, code, Guid.NewGuid());
        var second = await AcceptInviteAsync(inviteeClient, code, Guid.NewGuid());

        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var members = await ownerClient.GetFromJsonAsync<PaginatedItems<MemberDto>>($"/api/servers/{server.Id}/members");
        Assert.NotNull(members);
        Assert.Equal(2, members.Items.Count());
    }

    [Fact]
    public async Task CreateMessage_ByMember_ReturnsCreatedMessage()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);
        var channel = TextChannel(server);

        var message = await PostMessageAsync(client, channel.Id, "hello world");

        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(channel.Id, message.ChannelId);
        Assert.Equal(_ownerId, message.AuthorId);
        Assert.Equal("hello world", message.Text);
        Assert.False(message.IsEdited);
        Assert.Empty(message.Attachments);
    }

    [Fact]
    public async Task CreateMessage_WithAttachment_ResolvesAttachmentUrl()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);
        var channel = TextChannel(server);
        var fileId = Guid.NewGuid();

        var message = await PostMessageAsync(client, channel.Id, "with file", fileId);

        var attachment = Assert.Single(message.Attachments);
        Assert.Equal(fileId, attachment.FileId);
        Assert.Equal("attachment.png", attachment.FileName);
        Assert.EndsWith($"/{fileId}/content", attachment.Url);
    }

    [Fact]
    public async Task CreateMessage_ByNonMember_ReturnsForbidden()
    {
        var server = await CreateServerAsync(CreateClient(_ownerId));
        var channel = TextChannel(server);

        var response = await CreateClient(Guid.NewGuid())
            .PostAsJsonAsync($"/api/channels/{channel.Id}/messages", new { text = "hello" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMessages_ReturnsNewestFirstAndPagesWithCursor()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);
        var channel = TextChannel(server);

        var ids = new List<Guid>();
        for (var index = 0; index < 5; index++)
        {
            ids.Add((await PostMessageAsync(client, channel.Id, $"message {index}")).Id);

            // Message ids are only ordered across milliseconds, messages posted within the same one may swap places.
            await Task.Delay(5);
        }

        var firstPage = await GetMessagesAsync(client, channel.Id, limit: 2);

        Assert.Equal(new[] { ids[4], ids[3] }, firstPage.Items.Select(message => message.Id));
        Assert.True(firstPage.HasPrev);
        Assert.False(firstPage.HasNext);
        Assert.Equal(ids[3], firstPage.PrevCursor);
        Assert.Equal(ids[4], firstPage.NextCursor);

        var secondPage = await GetMessagesAsync(client, channel.Id, limit: 2, cursor: firstPage.PrevCursor);

        Assert.Equal(new[] { ids[2], ids[1] }, secondPage.Items.Select(message => message.Id));
        Assert.True(secondPage.HasPrev);

        var newerPage = await GetMessagesAsync(client, channel.Id, limit: 2, cursor: ids[1], older: false);

        Assert.Equal(new[] { ids[4], ids[3] }, newerPage.Items.Select(message => message.Id));
        Assert.True(newerPage.HasNext);
    }

    [Fact]
    public async Task EditMessage_ByAuthor_UpdatesTextAndFlagsItAsEdited()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);
        var channel = TextChannel(server);
        var message = await PostMessageAsync(client, channel.Id, "original");

        var response = await client.PatchAsJsonAsync($"/api/channels/{channel.Id}/messages/{message.Id}", new { text = "edited" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = await GetMessagesAsync(client, channel.Id, limit: 10);
        var stored = Assert.Single(page.Items);
        Assert.Equal("edited", stored.Text);
        Assert.True(stored.IsEdited);
    }

    [Fact]
    public async Task EditMessage_ByAnotherUser_DoesNotChangeText()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);
        var channel = TextChannel(server);
        var message = await PostMessageAsync(client, channel.Id, "original");

        await CreateClient(Guid.NewGuid())
            .PatchAsJsonAsync($"/api/channels/{channel.Id}/messages/{message.Id}", new { text = "hacked" });

        var page = await GetMessagesAsync(client, channel.Id, limit: 10);
        var stored = Assert.Single(page.Items);
        Assert.Equal("original", stored.Text);
        Assert.False(stored.IsEdited);
    }

    [Fact]
    public async Task DeleteServer_ByNonOwner_ReturnsForbidden()
    {
        var server = await CreateServerAsync(CreateClient(_ownerId));

        var response = await CreateClient(Guid.NewGuid()).DeleteAsync($"/api/servers/{server.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteServer_ByOwner_RemovesServer()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);

        var deleteResponse = await client.DeleteAsync($"/api/servers/{server.Id}");
        var getResponse = await client.GetAsync($"/api/servers/{server.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateMessage_WritesIntegrationEventToOutbox()
    {
        var client = CreateClient(_ownerId);
        var server = await CreateServerAsync(client);
        var channel = TextChannel(server);

        var message = await PostMessageAsync(client, channel.Id, "outbox");

        using var scope = Fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChannelContext>();
        var entries = await context.Set<IntegrationEventLogEntry>()
            .AsNoTracking()
            .Where(entry => entry.EventTypeName.EndsWith(nameof(MessageCreatedIntegrationEvent)))
            .ToListAsync();

        var entry = Assert.Single(entries, item => item.Content.Contains(message.Id.ToString()));
        Assert.Equal(EventStateEnum.Published, entry.State);
        Assert.Equal(1, entry.TimesSent);
    }
}
