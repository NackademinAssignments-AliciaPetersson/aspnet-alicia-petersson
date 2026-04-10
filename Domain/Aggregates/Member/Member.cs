using Domain.Common.Validators;

namespace Domain.Aggregates.Member;

public sealed class Member
{
    private Member(string id, string userId, string? firstName, string? lastName, string? profileImageUrl)
    {
        Id = id;
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        ProfileImageUrl = profileImageUrl;
    }

    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    public static Member Create(string userId, string? firstName = null, string? lastName = null, string? profileImageUrl = null)
    {
        return new(Guid.NewGuid().ToString(), userId, firstName, lastName, profileImageUrl);
    }

    public static Member Rehydrated(string id, string userId, string? firstName = null, string? lastName = null, string? profileImageUrl = null)
    {
        return new(id, userId, firstName, lastName, profileImageUrl);
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
}
