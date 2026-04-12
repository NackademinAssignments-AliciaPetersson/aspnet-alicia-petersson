using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;

namespace Tests.Unit.Members;

public class Membership_Tests
{
    private static MembershipType CreateValidType()
        => MembershipType.Create("Gold", 100);

    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var type = CreateValidType();

        var membership = Membership.Create(type, DateTime.UtcNow, 100);

        Assert.NotNull(membership);
        Assert.True(membership.IsActive);
    }

    [Fact]
    public void Create_ShouldThrow_WhenStartDateInPast()
    {
        var type = CreateValidType();

        Assert.Throws<ValidationDomainException>(() =>
            Membership.Create(type, DateTime.UtcNow.AddDays(-1), 100));
    }

    [Fact]
    public void Create_ShouldThrow_WhenMembershipTypeIsNull()
    {
        Assert.Throws<ValidationDomainException>(() =>
            Membership.Create(null!, DateTime.UtcNow.AddDays(-1), 100));
    }

    [Fact]
    public void Create_ShouldThrow_WhenPriceNegative()
    {
        var type = CreateValidType();

        Assert.Throws<ValidationDomainException>(() =>
            Membership.Create(type, DateTime.UtcNow, -10));
    }
}
