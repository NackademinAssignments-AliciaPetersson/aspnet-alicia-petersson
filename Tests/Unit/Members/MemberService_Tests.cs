using Application.Abstractions.Identity;
using Application.Abstractions.Persistence;
using Application.Common.Outputs;
using Application.Common.Results;
using Application.Modules.Members;
using Application.Modules.Members.Inputs;
using Domain.Abstractions.Logging;
using Domain.Aggregates.Members;
using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;
using Moq;

namespace Tests.Unit.Members;

public class MemberService_Tests
{
    private readonly Mock<IAuthService> _auth = new();
    private readonly Mock<ILogger> _logger = new();
    private readonly Mock<IMemberRepository> _repo = new();
    private readonly Mock<IAccountService> _account = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMembershipTypeRepository> _typeRepo = new();

    private readonly MemberService _service;

    public MemberService_Tests()
    {
        _service = new MemberService(
            _auth.Object,
            _logger.Object,
            _repo.Object,
            _account.Object,
            _uow.Object,
            _typeRepo.Object
        );
    }
    // --- CREATE MEMBER
    [Fact]
    public async Task CreateMemberAsync_ShouldReturnBadRequest_WhenInputIsNull()
    {
        var result = await _service.CreateMemberAsync(null!);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task CreateMemberAsync_ShouldReturnConflict_WhenUserExists()
    {
        _auth.Setup(x => x.DoesUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _service.CreateMemberAsync(
            new CreateMemberInput("test@test.com", "password"));

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.Conflict, result.ErrorType);
    }

    [Fact]
    public async Task CreateMemberAsync_ShouldCreateMember_WhenValid()
    {
        _auth.Setup(x => x.DoesUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _auth.Setup(x => x.SignUpLocalUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<string?>.Ok(Guid.NewGuid().ToString()));

        _repo.Setup(x => x.AddAsync(It.IsAny<Member>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member m, CancellationToken _) => m);

        _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

        var result = await _service.CreateMemberAsync(
            new CreateMemberInput("test@test.com", "password"));

        Assert.True(result.Success);
    }

    // -- CREATE EXTERNAL MEMBER
    [Fact]
    public async Task CreateMemberForExternalUserAsync_ShouldReturnBadRequest_WhenInputIsNull()
    {
        var result = await _service.CreateMemberForExternalUserAsync(null!);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task CreateMemberForExternalUserAsync_ShouldReturnBadRequest_WhenEmailIsNull()
    {
        var input = new CreateExternalMemberInput(Guid.NewGuid().ToString(), null);

        var result = await _service.CreateMemberForExternalUserAsync(input);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task CreateMemberForExternalUserAsync_ShouldReturnError_WhenRepoReturnsNull()
    {
        var input = new CreateExternalMemberInput(Guid.NewGuid().ToString(), "test@test.com");

        _repo.Setup(x => x.AddAsync(It.IsAny<Member>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _service.CreateMemberForExternalUserAsync(input);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.Unexpected, result.ErrorType);
    }

    [Fact]
    public async Task CreateMemberForExternalUserAsync_ShouldReturnOk_WhenValidInput()
    {
        var input = new CreateExternalMemberInput(Guid.NewGuid().ToString(), "test@test.com", "John", "Doe", "img.png");

        _repo.Setup(x => x.AddAsync(It.IsAny<Member>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member m, CancellationToken _) => m);

        _uow.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateMemberForExternalUserAsync(input);

        Assert.True(result.Success);
    }

    // --- DELETE MEMBER
    [Fact]
    public async Task DeleteMemberAsync_ShouldReturnBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _service.DeleteMemberAsync("");

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task DeleteMemberAsync_ShouldReturnNotFound_WhenMemberMissing()
    {
        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _service.DeleteMemberAsync("user");

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task DeleteMemberAsync_ShouldDeleteMember_WhenValid()
    {
        var member = Member.Create(Guid.NewGuid().ToString());

        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        _repo.Setup(x => x.RemoveByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _account.Setup(x => x.DeleteAuthenticationUserAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Ok());

        _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

        var result = await _service.DeleteMemberAsync(member.UserId);

        Assert.True(result.Success);
    }

    // -- GET MEMBER DETAILS
    [Fact]
    public async Task GetMemberDetailsAsync_ShouldThrow_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<NullDomainException>(() =>
            _service.GetMemberDetailsAsync(""));
    }
    [Fact]
    public async Task GetMemberDetailsAsync_ShouldReturnNotFound_WhenMemberMissing()
    {
        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _service.GetMemberDetailsAsync("user");

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetMemberDetailsAsync_ShouldReturnNotFound_WhenAuthFails()
    {
        var member = Member.Create(Guid.NewGuid().ToString());

        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        _account.Setup(x => x.GetAuthenticationUserDetailsAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<AuthenticationUserDetails?>.Error("fail"));

        var result = await _service.GetMemberDetailsAsync(member.UserId);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetMemberDetailsAsync_ShouldReturnOk_WhenValid()
    {
        var member = Member.Create(Guid.NewGuid().ToString());

        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        _account.Setup(x => x.GetAuthenticationUserDetailsAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<AuthenticationUserDetails?>.Ok(
                new AuthenticationUserDetails(Guid.NewGuid().ToString(),"test@test.com", "123")));

        var result = await _service.GetMemberDetailsAsync(member.UserId);

        Assert.True(result.Success);
    }

    // --- UPDATE MEMBER DETAILS
    [Fact]
    public async Task UpdateMemberDetailsAsync_ShouldThrow_WhenInputIsNull()
    {
        await Assert.ThrowsAsync<NullDomainException>(() =>
            _service.UpdateMemberDetailsAsync(null!));
    }

    [Fact]
    public async Task UpdateMemberDetailsAsync_ShouldReturnNotFound_WhenMemberMissing()
    {
        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _service.UpdateMemberDetailsAsync(
            new UpdateMemberDetailsInput("user", "John", "Doe", "img.png", "123"));

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task UpdateMemberDetailsAsync_ShouldReturnOk_WhenValid()
    {
        var member = Member.Create(Guid.NewGuid().ToString());

        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        _repo.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Member>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

        _account.Setup(x => x.UpdateAuthenticationUserDetailsAsync(It.IsAny<UpdateAuthenticationUserDetailsInput>()))
            .ReturnsAsync(Result.Ok());

        var result = await _service.UpdateMemberDetailsAsync(
            new UpdateMemberDetailsInput("user", "John", "Doe", "img.png", "123"));

        Assert.True(result.Success);
    }

    // --- SET MEMBERSHIP
    [Fact]
    public async Task SetMembershipAsync_ShouldReturnBadRequest_WhenInputNull()
    {
        var result = await _service.SetMembershipAsync(null!);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.BadRequest, result.ErrorType);
    }
    [Fact]
    public async Task SetMembershipAsync_ShouldReturnNotFound_WhenMemberMissing()
    {
        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _service.SetMembershipAsync(
            new SetMembershipInput("user", 1));

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    // --- GET MEMBERSHIP DETAILS
    [Fact]
    public async Task GetMembershipDetailsAsync_ShouldReturnBadRequest_WhenUserIdEmpty()
    {
        var result = await _service.GetMembershipDetailsAsync("");

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task GetMembershipDetailsAsync_ShouldReturnNotFound_WhenMemberMissing()
    {
        _repo.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _service.GetMembershipDetailsAsync("user");

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }
}
