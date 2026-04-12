using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;

namespace Domain.Aggregates.Members;

public sealed class Member
{
    private Member(string id, string userId, string? firstName, string? lastName, string? profileImageUrl, IReadOnlyCollection<Membership> memberships)
    {
        Id = id;
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        ProfileImageUrl = profileImageUrl;
        _memberships = [.. memberships];
    }

    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    private readonly List<Membership> _memberships = [];
    public IReadOnlyCollection<Membership> Memberships => _memberships.AsReadOnly();
    public Membership? CurrentMembership => _memberships.OrderByDescending(membership => membership.StartDateUtc).FirstOrDefault(membership => membership.IsActive);


    public static Member Create(string userId, string? firstName = null, string? lastName = null, string? profileImageUrl = null)
    {
        return new(Guid.NewGuid().ToString(), userId, firstName, lastName, profileImageUrl, []);
    }

    public static Member Rehydrated(string id, string userId, string? firstName = null, string? lastName = null, string? profileImageUrl = null, IReadOnlyCollection<Membership>? memberships = null)
    {
        return new(id, userId, firstName, lastName, profileImageUrl, memberships ?? []);
    }

    public void UpdateDetailsInformation(string? newFirstName, string? newLastName, string? newProfileImageUrl)
    {
        if (!string.IsNullOrWhiteSpace(newFirstName))
            FirstName = newFirstName;

        if (!string.IsNullOrWhiteSpace(newLastName))
            LastName = newLastName;
        
        if (!string.IsNullOrWhiteSpace(newProfileImageUrl))
            ProfileImageUrl = newProfileImageUrl;
    }

    public Membership AcquireMembership(MembershipType membershipType)
    {
        if (membershipType is null)
            throw new ValidationDomainException("Membership Type is required");

        if (CurrentMembership is not null)
            throw new ValidationDomainException("Can't aquire new membership when member already has an active membership");

        var membership = Membership.Create(membershipType, DateTime.UtcNow, membershipType.BasePrice);        

        _memberships.Add(membership);        

        return membership;
    }    

    public void CancelMembership(Membership membership)
    {
        if (CurrentMembership is null)
            throw new ValidationDomainException("Can't cancel membership when member has no active membership");

        if (CurrentMembership.Id != membership.Id)
            throw new ValidationDomainException("Can't cancel a membership that is not the current active membership");

        if (_memberships.Any(ms => ms.Id == membership.Id))
            throw new ValidationDomainException("Membership does not belong to this member");

        membership.Deactivate();
    }
}
