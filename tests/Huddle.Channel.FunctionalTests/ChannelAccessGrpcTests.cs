using System.Net.Http.Json;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Huddle.Grpc;

namespace Huddle.Channel.FunctionalTests;

/// <summary>
/// The channel access gRPC endpoint is the contract the SignalR and voice services rely on, so it is exercised
/// through a real gRPC call against the in-memory server.
/// </summary>
[Collection(ChannelApiCollection.Name)]
public class ChannelAccessGrpcTests(ChannelApiFixture fixture) : ChannelApiTestBase(fixture)
{
    private static readonly Method<ChannelAccessRequest, ChannelAccessResponse> CheckChannelAccessMethod = new(
        MethodType.Unary,
        "ChannelAccess",
        "CheckChannelAccess",
        Marshallers.Create(
            (ChannelAccessRequest request) => request.ToByteArray(),
            (byte[] payload) => ChannelAccessRequest.Parser.ParseFrom(payload)),
        Marshallers.Create(
            (ChannelAccessResponse response) => response.ToByteArray(),
            (byte[] payload) => ChannelAccessResponse.Parser.ParseFrom(payload)));

    [Fact]
    public async Task CheckChannelAccess_ForMember_ReturnsTrue()
    {
        var memberId = Guid.NewGuid();
        var memberClient = CreateClient(memberId);
        var server = await CreateServerAsync(memberClient);
        var channel = TextChannel(server);

        var response = await CheckChannelAccessAsync(channel.Id, memberId);

        Assert.True(response.HasAccess);
    }

    [Fact]
    public async Task CheckChannelAccess_ForStranger_ReturnsFalse()
    {
        var server = await CreateServerAsync(CreateClient(Guid.NewGuid()));
        var channel = TextChannel(server);

        var response = await CheckChannelAccessAsync(channel.Id, Guid.NewGuid());

        Assert.False(response.HasAccess);
    }

    private async Task<ChannelAccessResponse> CheckChannelAccessAsync(Guid channelId, Guid identityId)
    {
        using var channel = CreateGrpcChannel();
        var invoker = channel.CreateCallInvoker();

        return await invoker.AsyncUnaryCall(
            CheckChannelAccessMethod,
            host: null,
            options: new CallOptions(),
            request: new ChannelAccessRequest
            {
                ChannelId = channelId.ToString(),
                IdentityId = identityId.ToString(),
            });
    }

    private GrpcChannel CreateGrpcChannel() =>
        GrpcChannel.ForAddress(Fixture.Server.BaseAddress, new GrpcChannelOptions
        {
            HttpHandler = Fixture.Server.CreateHandler(),
        });
}
