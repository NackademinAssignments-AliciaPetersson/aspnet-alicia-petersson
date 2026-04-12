using Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EFC.Configurations;

internal sealed class MembershipTypeConfiguration : IEntityTypeConfiguration<MembershipTypeEntity>
{
    public void Configure(EntityTypeBuilder<MembershipTypeEntity> builder)
    {
        builder.ToTable("MembershipTypes");

        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
        builder.Property(e => e.BasePrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        // Indexes
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UQ_MembershipTypes_Name");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_MembershipTypes_IsActive");

        //Checks
        builder.ToTable(tb => {
            tb.HasCheckConstraint("CK_MembershipTypes_BasePriceNotNegative", "[BasePrice] >= 0");
            tb.HasCheckConstraint("CK_MembershipTypes_NameNotEmpty", "LTRIM(RTRIM([Name])) <> ''");
        });
    }
}
