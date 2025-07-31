using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huddle.Channel.Infrastructure.EntityConfigurations
{
    public class ServerConfiguration : IEntityTypeConfiguration<Server>
    {
        public void Configure(EntityTypeBuilder<Server> serverConfiguration)
        {
            serverConfiguration.HasKey(x => x.Id);
            serverConfiguration.Property(e => e.Id).ValueGeneratedNever();

            serverConfiguration.Ignore(s => s.DomainEvents);

            serverConfiguration.Metadata.FindNavigation(nameof(Server.Channels))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
