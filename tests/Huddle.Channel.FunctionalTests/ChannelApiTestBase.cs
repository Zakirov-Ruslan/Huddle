using System.Net;
using System.Net.Http.Json;

namespace Huddle.Channel.FunctionalTests;

public abstract class ChannelApiTestBase
{
    protected ChannelApiTestBase(ChannelApiFixture fixture) => Fixture = fixture;

    protected ChannelApiFixture Fixture { get; }

    protected HttpClient CreateClient(Guid userId)
    {
        var client = Fixture.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation(TestAuthHandler.UserHeaderName, userId.ToString());

        return client;
    }

    protected static string NewServerName() => $"srv-{Guid.NewGuid():N}"[..20];

    protected async Task<ServerDto> CreateServerAsync(HttpClient client, string? name = null)
    {
        var response = await client.PostAsJsonAsync("/api/servers", new
        {
            name = name ?? NewServerName(),
            isPrivate = false,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var server = await response.Content.ReadFromJsonAsync<ServerDto>();
        Assert.NotNull(server);

        return server;
    }

    protected static ChannelDto TextChannel(ServerDto server) =>
        server.Channels.Single(channel => channel.ChannelType == "Text");

    protected async Task<MessageDto> PostMessageAsync(
        HttpClient client,
        Guid channelId,
        string text,
        params Guid[] attachmentIds)
    {
        var response = await client.PostAsJsonAsync($"/api/channels/{channelId}/messages", new
        {
            text,
            attachmentIds,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var message = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(message);

        return message;
    }

    protected static async Task<PaginatedItems<MessageDto>> GetMessagesAsync(
        HttpClient client,
        Guid channelId,
        int limit,
        Guid? cursor = null,
        bool older = true)
    {
        var url = $"/api/channels/{channelId}/messages?limit={limit}&older={older.ToString().ToLowerInvariant()}";
        if (cursor.HasValue)
            url += $"&cursor={cursor.Value}";

        var page = await client.GetFromJsonAsync<PaginatedItems<MessageDto>>(url);
        Assert.NotNull(page);

        return page;
    }

    protected static async Task<string> CreateInviteAsync(HttpClient client, Guid serverId)
    {
        var response = await client.PostAsync($"/api/servers/{serverId}/invites", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var invite = await response.Content.ReadFromJsonAsync<InviteDto>();
        Assert.NotNull(invite);

        return invite.Code;
    }

    protected static async Task<HttpResponseMessage> AcceptInviteAsync(HttpClient client, string code, Guid requestId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/invites/{code}/accept");
        request.Headers.Add("x-requestid", requestId.ToString());

        return await client.SendAsync(request);
    }
}
