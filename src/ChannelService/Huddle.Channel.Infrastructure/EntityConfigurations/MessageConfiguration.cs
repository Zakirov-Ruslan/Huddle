using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huddle.Channel.Infrastructure.EntityConfigurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> messageConfiguration)
        {
            messageConfiguration.HasKey(x => x.Id);
            messageConfiguration.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("uuid");

            messageConfiguration.Ignore(s => s.DomainEvents);

            // SentAt index doesnt need for pagination with v7 uuid
            //messageConfiguration.HasIndex(m => m.SentAt);

            messageConfiguration
                .HasOne<Domain.Aggregates.ServerAggregate.Channel>()
                .WithMany()
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
