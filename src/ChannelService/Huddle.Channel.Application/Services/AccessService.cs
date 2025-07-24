
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Application.Services
{
    public class AccessService : IAccessService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IServerRepository _serverRepository;

        public AccessService(IMemberRepository memberRepository, IServerRepository serverRepository)
        {
            _memberRepository = memberRepository;
            _serverRepository = serverRepository;
        }

        public async Task<bool> CanUserAccessChannelAsync(Guid channelId, Guid identityId)
        {
            var channel = await _serverRepository.GetChannelAsync(channelId)
                ?? throw new KeyNotFoundException("Channel not found");

            var member = await _memberRepository.GetByServerAndIdentityIdAsync(channel.ServerId, identityId);

            return member != null;
        }

        public async Task<bool> IsUserMemberOfServerAsync(Guid serverId, Guid identityId)
        {
            var member = await _memberRepository.GetByServerAndIdentityIdAsync(serverId, identityId);

            return member != null;
        }
    }
}
