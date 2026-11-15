using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Application.Mapping
{
    public interface IServerDtoFactory
    {
        ServerDto Create(Server server);
    }

    public class ServerDtoFactory : IServerDtoFactory
    {
        private readonly IFileUrlProvider _fileUrlProvider;

        public ServerDtoFactory(IFileUrlProvider fileUrlProvider)
        {
            _fileUrlProvider = fileUrlProvider;
        }

        public ServerDto Create(Server server) =>
            new(
                server.Id,
                server.Name,
                server.OwnerIdentityId,
                server.AvatarFileId.HasValue ? _fileUrlProvider.BuildUrl(server.AvatarFileId.Value) : null,
                server.Channels
                    .Select(channel => new ChannelDto(channel.Id, channel.ServerId, channel.Name, channel.Type.ToString()))
                    .ToList());
    }
}
