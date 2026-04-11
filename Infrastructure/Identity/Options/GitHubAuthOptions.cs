namespace Infrastructure.Identity.Options;

public sealed class GitHubAuthOptions
{
    public const string SectionName = "Authentication:GitHub";

    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
}
