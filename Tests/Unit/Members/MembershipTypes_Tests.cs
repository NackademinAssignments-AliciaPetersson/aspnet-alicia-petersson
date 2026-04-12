using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;

namespace Tests.Unit.Members;

public class MembershipTypes_Tests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var type = MembershipType.Create("Gold", 100);

        Assert.Equal("Gold", type.Name);
        Assert.Equal(100, type.BasePrice);
        Assert.True(type.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Create_ShouldThrow_WhenNameIsInvalid(string? name)
    {
        Assert.Throws<ValidationDomainException>(() =>
            MembershipType.Create(name!, 100));
    }

    [Fact]
    public void Create_ShouldThrow_WhenPriceIsNegative()
    {
        Assert.Throws<ValidationDomainException>(() =>
            MembershipType.Create("Gold", -1));
    }

    [Fact]
    public void Rehydrate_ShouldSetAllProperties()
    {
        var type = MembershipType.Rehydrate(1, "Gold", 100, true);

        Assert.Equal(1, type.Id);
        Assert.Equal("Gold", type.Name);
        Assert.Equal(100, type.BasePrice);
        Assert.True(type.IsActive);
    }
}
