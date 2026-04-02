using Domain.Aggregates.Member;
using Infrastructure.Identity;
using Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EFC.Configurations;

internal sealed class MemberConfiguration : IEntityTypeConfiguration<MemberEntity>
{
    public void Configure(EntityTypeBuilder<MemberEntity> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(e => e.Id).HasName("PK_Members_Id");

        // Properties
        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.FirstName);

        builder.Property(e => e.LastName);

        builder.Property(e => e.ProfileImageUrl);

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2(0)");

        builder.Property(e => e.UpdatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2(0)");

        builder.Property(e => e.RowVersion)
            .IsRequired(false)
            .IsRowVersion()
            .IsConcurrencyToken();


        // Indexes
        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_Members_UserId");

        //Checks
        builder.ToTable(tb => {
            tb.HasCheckConstraint("CK_Members_IdNotEmpty","LTRIM(RTRIM([Id])) <> ''");
            tb.HasCheckConstraint("CK_Members_UserIdNotEmpty", "LTRIM(RTRIM([UserId])) <> ''");
        });

        //Relations
        builder.HasOne(member => member.User)
            .WithOne()
            .HasForeignKey<MemberEntity>(member => member.UserId)
            .HasPrincipalKey<AuthenticationUser>(user => user.Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
