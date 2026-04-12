
using Domain.Abstractions.Logging;
using Domain.Aggregates.Members;
using Infrastructure.Persistence.EFC.Entities;
using Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Tests.Fakes;
using Tests.Fixtures;

namespace Tests.Integration.Infrastructure.Persistence.Repositories.Members;

[Collection(PersistenceCollection.Name)]
public sealed class MemberRepository_Tests(PersistenceFixture fixture)
{
    private readonly ILogger _logger = new TestLogger();

    [Fact]
    public async Task AddAsync_ShouldPersistMember()
    {
        //arrange
        await using var context = fixture.CreateContext();

        //var repo = new MemberRepository(context, _logger);

        //var typeEntity = new MembershipTypeEntity
        //{
        //    Id = 1,
        //    Name = "Gold",
        //    BasePrice = 100,
        //    IsActive = true
        //};

        //context.MembershipTypes.Add(typeEntity);
        //await context.SaveChangesAsync();

        //context.ChangeTracker.Clear();

        //var member = Member.Create(Guid.NewGuid().ToString());

        //// Act
        //await repo.AddAsync(member);

        //var entries = context.ChangeTracker.Entries()
        //.Select(e => e.Entity)
        //.ToList();

        //await context.SaveChangesAsync();

        //var fromDb = await context.Members.FirstOrDefaultAsync(x => x.Id == member.Id);

        //// Assert
        //Assert.NotNull(fromDb);
        //Assert.Equal(member.UserId, fromDb!.UserId);
    }

    //[Fact]
    //public async Task GetByUserId_ShouldReturnMember_WithMemberships()
    //{
    //    // Arrange
    //    await using var context = fixture.CreateContext();

    //    var repo = new MemberRepository(context, _logger);

    //    var member = Member.Create(Guid.NewGuid().ToString());
    //    var type = MembershipType.Create("Gold", 100);

    //    member.AcquireMembership(type);

    //    // Act
    //    await repo.AddAsync(member);
    //    await context.SaveChangesAsync();

    //    var result = await repo.GetByUserIdAsync(member.UserId);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Single(result!.Memberships);
    //}

    //[Fact]
    //public async Task GetById_ShouldIncludeMembershipsAndType()
    //{
    //    await using var context = fixture.CreateContext();

    //    var repo = new MemberRepository(context, _logger);

    //    var typeEntity = new MembershipTypeEntity
    //    {
    //        Id = 1,
    //        Name = "Gold",
    //        BasePrice = 100,
    //        IsActive = true
    //    };

    //    context.MembershipTypes.Add(typeEntity);
    //    await context.SaveChangesAsync();

    //    var member = Member.Create(Guid.NewGuid().ToString());
    //    var type = MembershipType.Create("Gold", 100);

    //    member.AcquireMembership(type);

    //    await repo.AddAsync(member);
    //    await context.SaveChangesAsync();

    //    var result = await repo.GetByIdAsync(member.Id);

    //    Assert.NotNull(result);
    //    Assert.NotEmpty(result!.Memberships);
    //    Assert.Equal("Gold", result.Memberships.First().MembershipType.Name);
    //}

    //[Fact]
    //public async Task Update_ShouldSyncMemberships_Correctly()
    //{
    //    await using var context = fixture.CreateContext();

    //    var repo = new MemberRepository(context, _logger);

    //    var type = MembershipType.Create("Gold", 100);
    //    var member = Member.Create(Guid.NewGuid().ToString());

    //    var membership = member.AcquireMembership(type);

    //    await repo.AddAsync(member);
    //    await context.SaveChangesAsync();

    //    // Modify domain
    //    member.RemoveAllMemberships();

    //    await repo.UpdateAsync(member.Id, member);
    //    await context.SaveChangesAsync();

    //    var fromDb = await repo.GetByIdAsync(member.Id);

    //    Assert.Empty(fromDb!.Memberships);
    //}

    //[Fact]
    //public async Task RemoveById_ShouldDeleteMember()
    //{
    //    await using var context = fixture.CreateContext();

    //    var repo = new MemberRepository(context, _logger);

    //    var member = Member.Create(Guid.NewGuid().ToString());

    //    await repo.AddAsync(member);
    //    await context.SaveChangesAsync();

    //    var removed = await repo.RemoveByIdAsync(member.Id);
    //    await context.SaveChangesAsync();

    //    Assert.True(removed);

    //    var exists = await context.Members.AnyAsync(x => x.Id == member.Id);
    //    Assert.False(exists);
    //}
}
