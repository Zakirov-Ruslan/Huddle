using Huddle.Channel.Domain.Aggregates.Events;
using Huddle.Channel.Domain.Aggregates.Events.Server;
using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.ServerAggregate
{
    public class Server : Entity, IAggregateRoot
    {
        const int MAX_NAME_LENGTH = 30;

        public string Name { get; private set; }
        public Guid OwnerIdentityId { get; private set; }
        public bool IsPrivate { get; private set; }

        private readonly List<Channel> _channels = [];
        public IEnumerable<Channel> Channels => _channels.AsReadOnly();

        private Server() { }

        public Server(Guid ownerId, string name, bool isPrivate)
        {
            ValidateName(name);

            Id = Guid.NewGuid();
            OwnerIdentityId = ownerId;
            Name = name;
            IsPrivate = isPrivate;

            ServerCreatedDomainEvent @event = new(Id, Name, OwnerIdentityId);
            AddDomainEvent(@event);
        }

        public void UpdateName(string name)
        {
            ValidateName(name);

            Name = name;

            ServerUpdatedDomainEvent @event = new(Id, name);
            AddDomainEvent(@event);
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            if (name.Length > MAX_NAME_LENGTH)
                throw new ArgumentException(nameof(name));
        }

        public Channel AddChannel(ChannelType channelType, string name)
        {
            var channel = new Channel(name, channelType, this);
            _channels.Add(channel);

            AddDomainEvent(new ChannelCreatedDomainEvent(channel.Id, channel.Name, channel.Type, channel.ServerId));

            return channel;
        }
        
        public void DeleteChannel(Guid id)
        {
            var channel = _channels.First(c => c.Id == id);
            _channels.Remove(channel);
            AddDomainEvent(new ChannelDeletedDomainEvent(id, Id));
        }
    }
}
