using Grpc.Core;
using Huddle.Channel.Application.Services;
using Huddle.Grpc;

namespace Huddle.Channel.WebApi.Grpc
{
    public class ChannelAccessGrpcService : ChannelAccess.ChannelAccessBase
    {
        private readonly IAccessService _accessService;

        public ChannelAccessGrpcService(IAccessService accessService)
        {
            _accessService = accessService;
        }

        public override async Task<ChannelAccessResponse> CheckChannelAccess(
            ChannelAccessRequest request,
            ServerCallContext context)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(
                Guid.Parse(request.ChannelId),
                Guid.Parse(request.IdentityId));

            return new ChannelAccessResponse { HasAccess = hasAccess };
        }

        public override async Task<ServerAccessResponse> CheckServerAccess(
            ServerAccessRequest request,
            ServerCallContext context)
        {
            var hasAccess = await _accessService.IsUserMemberOfServerAsync(
                Guid.Parse(request.ServerId),
                Guid.Parse(request.IdentityId));

            return new ServerAccessResponse { HasAccess = hasAccess };
        }
    }
}
