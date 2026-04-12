using Domain.Aggregates.Members;
using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;

namespace Tests.Unit.Members;

public class Member_Tests
{
    private static MembershipType CreateValidType()
        => MembershipType.Create("Standard", 100);

    [Fact]
    public void Create_ShouldInitializeMember()
    {
        var userId = Guid.NewGuid().ToString();

        var member = Member.Create(userId);

        Assert.NotNull(member);
        Assert.Equal(userId, member.UserId);
        Assert.Empty(member.Memberships);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_ShouldThrow_WhenUserIdIsInvalid(string? userId)
    {
        Assert.Throws<ValidationDomainException>(() =>
            Member.Create(userId!));
    }

    [Fact]
    public void Rehydrated_ShouldCreate_WhenValidData()
    {
        var id = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();

        var member = Member.Rehydrated(id, userId);

        Assert.Equal(id, member.Id);
        Assert.Equal(userId, member.UserId);
    }

    [Fact]
    public void AcquireMembership_ShouldAddMembership_WhenNoneExists()
    {
        var member = Member.Create(Guid.NewGuid().ToString());
        var type = CreateValidType();

        var membership = member.AcquireMembership(type);

        Assert.Single(member.Memberships);
        Assert.Equal(membership, member.CurrentMembership);
    }

    [Fact]
    public void AcquireMembership_ShouldThrow_WhenActiveMembershipExists()
    {
        var member = Member.Create(Guid.NewGuid().ToString());
        var type = CreateValidType();

        member.AcquireMembership(type);

        Assert.Throws<ValidationDomainException>(() =>
            member.AcquireMembership(type));
    }

    [Fact]
    public void AcquireMembership_ShouldThrow_WhenTypeIsNull()
    {
        var member = Member.Create(Guid.NewGuid().ToString());

        Assert.Throws<ValidationDomainException>(() =>
            member.AcquireMembership(null!));
    }

    [Fact]
    public void CancelMembership_ShouldDeactivate_WhenValid()
    {
        var member = Member.Create(Guid.NewGuid().ToString());
        var type = CreateValidType();

        var membership = member.AcquireMembership(type);

        member.CancelMembership(membership);

        Assert.False(membership.IsActive);
    }

    [Fact]
    public void CancelMembership_ShouldThrow_WhenNoActiveMembership()
    {
        var member = Member.Create(Guid.NewGuid().ToString());
        var type = CreateValidType();

        var membership = Membership.Create(type, DateTime.UtcNow, 100);

        Assert.Throws<ValidationDomainException>(() =>
            member.CancelMembership(membership));
    }

    [Fact]
    public void CancelMembership_ShouldThrow_WhenWrongMembership()
    {
        var member = Member.Create(Guid.NewGuid().ToString());
        var type = CreateValidType();

        var active = member.AcquireMembership(type);
        var other = Membership.Create(type, DateTime.UtcNow, 100);

        Assert.Throws<ValidationDomainException>(() =>
            member.CancelMembership(other));
    }

    [Fact]
    public void UpdateDetailsInformation_ShouldUpdateOnlyNonEmptyValues()
    {
        var member = Member.Create(Guid.NewGuid().ToString());

        member.UpdateDetailsInformation("John", null, "");

        Assert.Equal("John", member.FirstName);
        Assert.Null(member.LastName);
        Assert.Null(member.ProfileImageUrl);
    }

    [Fact]
    public void RemoveAllMemberships_ShouldClearList()
    {
        var member = Member.Create(Guid.NewGuid().ToString());
        var type = CreateValidType();

        member.AcquireMembership(type);

        member.RemoveAllMemberships();

        Assert.Empty(member.Memberships);
    }
}
