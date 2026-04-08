using Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EFC.Configurations;

internal class ContactRequestConfiguration : IEntityTypeConfiguration<ContactRequestEntity>
{
    public void Configure(EntityTypeBuilder<ContactRequestEntity> builder)
    {
        builder.ToTable("ContactRequests");

        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .IsRequired();

        builder.Property(x => x.LastName)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.PhoneNumber);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2(0)");

        // Indexes
        builder.HasIndex(x => x.CreatedAtUtc);

    }
}
