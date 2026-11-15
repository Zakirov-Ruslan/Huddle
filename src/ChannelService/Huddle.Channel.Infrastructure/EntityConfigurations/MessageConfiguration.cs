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

            messageConfiguration.HasIndex(x => new { x.ChannelId, x.Id });

            messageConfiguration.Ignore(s => s.DomainEvents);

            messageConfiguration.OwnsMany(message => message.Attachments, attachment =>
            {
                attachment.ToTable("message_attachments");
                attachment.WithOwner().HasForeignKey("MessageId");
                attachment.HasKey(a => new { a.FileId, a.Position });

                attachment.Property(a => a.FileName).HasMaxLength(255).IsRequired();
                attachment.Property(a => a.ContentType).HasMaxLength(150).IsRequired();
            });

            messageConfiguration.Metadata.FindNavigation(nameof(Message.Attachments))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);

            messageConfiguration
                .HasOne<Domain.Aggregates.ServerAggregate.Channel>()
                .WithMany()
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
