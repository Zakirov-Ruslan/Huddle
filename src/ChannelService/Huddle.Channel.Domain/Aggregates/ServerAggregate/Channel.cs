using Huddle.Channel.Domain.Aggregates.Events;
using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.ServerAggregate
{
    public class Channel : Entity
    {
        const int MAX_NAME_LENGTH = 30;

        public string Name { get; private set; }
        public ChannelType Type { get; private set; } 
        public Guid ServerId { get; private set; }
        public Server Server { get; private set; }

        private Channel() { }

        internal Channel(string name, ChannelType channelType, Server server)
        {
            ValidateName(name);

            Name = name;
            Type = channelType;
            Server = server;
        }

        public void ChangeName(string name)
        {
            ValidateName(name);

            Name = name;

            AddDomainEvent(new ChannelUpdatedDomainEvent(Id, name));
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            if (name.Length > MAX_NAME_LENGTH)
                throw new ArgumentException(nameof(name));
        }
    }
}
