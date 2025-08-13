using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huddle.Channel.Infrastructure.EntityConfigurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> memberConfiguration)
        {
            memberConfiguration.HasKey(x => x.Id);

            memberConfiguration
                .HasIndex(m => new { m.ServerId, m.IdentityId })
                .IsUnique(true);

            memberConfiguration.Ignore(s => s.DomainEvents);

            memberConfiguration
                .HasOne<Server>()
                .WithMany()
                .HasForeignKey(m => m.ServerId)
                .OnDelete(DeleteBehavior.Cascade);

            memberConfiguration
                .OwnsOne(m => m.Profile);
        }
    }
}
