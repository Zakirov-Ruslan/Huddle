using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huddle.Channel.Infrastructure.EntityConfigurations
{
    public class InviteConfiguration : IEntityTypeConfiguration<Invite>
    {
        public void Configure(EntityTypeBuilder<Invite> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Ignore(i => i.DomainEvents);

            builder.HasOne<Server>()
                .WithMany()
                .HasForeignKey(i => i.ServerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.ServerId)
                .IsUnique(false); 

            builder.HasIndex(i => i.Code)
                .IsUnique(true);
        }
    }
}
