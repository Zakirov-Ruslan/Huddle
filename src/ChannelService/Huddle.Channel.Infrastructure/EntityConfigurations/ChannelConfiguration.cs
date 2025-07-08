using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huddle.Channel.Infrastructure.EntityConfigurations
{
    public class ChannelConfiguration : IEntityTypeConfiguration<Domain.Aggregates.ServerAggregate.Channel>
    {
        public void Configure(EntityTypeBuilder<Domain.Aggregates.ServerAggregate.Channel> channelConfiguration)
        {
            channelConfiguration.HasKey(x => x.Id);

            channelConfiguration.Ignore(s => s.DomainEvents);

            channelConfiguration
                .Property(o => o.Type)
                .HasConversion<string>();

            channelConfiguration.HasOne(c => c.Server)
                .WithMany(s => s.Channels)
                .HasForeignKey(c => c.ServerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
