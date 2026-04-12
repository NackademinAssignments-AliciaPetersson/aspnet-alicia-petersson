using Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EFC.Configurations;

internal sealed class MembershipConfiguration : IEntityTypeConfiguration<MembershipEntity>
{
    public void Configure(EntityTypeBuilder<MembershipEntity> builder)
    {
        builder.ToTable("Memberships");

        builder.HasKey(e => e.Id).HasName("PK_Memberships_Id");

        // Properties
        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.MemberId)
            .IsRequired();

        builder.Property(e => e.MembershipTypeId)
            .IsRequired();

        builder.Property(e => e.StartDateUtc)
            .IsRequired()
            .HasColumnType("datetime2(0)");

        builder.Property(e => e.EndDateUtc)
            .HasColumnType("datetime2(0)");

        builder.Property(e => e.MonthlyPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Indexes
        builder.HasIndex(e => new { e.MemberId, e.MembershipTypeId })
            .IsUnique()
            .HasDatabaseName("UQ_Memberships_MemberId_MembershipTypeId");

        // Checks
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Memberships_IdNotEmpty", "LTRIM(RTRIM([Id])) <> ''");
            tb.HasCheckConstraint("CK_Memberships_MemberIdNotEmpty", "LTRIM(RTRIM([MemberId])) <> ''");
            tb.HasCheckConstraint("CK_Memberships_StartDateBeforeEndDate",
                "[EndDateUtc] IS NULL OR [StartDateUtc] <= [EndDateUtc]");
        });

        //Relations
        builder.HasOne(membership => membership.Member)
            .WithMany(member => member.Memberships)
            .HasForeignKey(membership => membership.MemberId)
            .HasConstraintName("FK_Memberships_Members_MemberId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(membership => membership.MembershipType)
            .WithMany()
            .HasForeignKey(membership => membership.MembershipTypeId)
            .HasConstraintName("FK_Memberships_MembershipTypes_MembershipTypeId")
            .OnDelete(DeleteBehavior.Restrict);        
    }
}
