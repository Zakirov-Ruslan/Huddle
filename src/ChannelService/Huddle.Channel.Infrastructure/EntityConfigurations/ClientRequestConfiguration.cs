using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Huddle.Channel.Infrastructure.Idempotency;

namespace Huddle.Channel.Infrastructure.EntityConfigurations;

public class ClientRequestConfiguration : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> builder)
    {
        builder.ToTable("ClientRequests");
        builder.HasKey(cr => cr.Id);
        builder.Property(cr => cr.Name)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(cr => cr.Time)
            .IsRequired();
    }
}
