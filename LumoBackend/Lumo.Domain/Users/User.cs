using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Users;

/// <summary>
/// Represents a user in the system.
/// /// This class contains properties related to the user's identity, profile, and social links.
/// </summary>
public class User : Entity
{
    public string IdentityId { get; private set; }
    public Name UserName { get; private set; }
    public EmailAddress Email { get; private set; }
    public string Bio { get; private set; } = string.Empty;
    public string ProfilePictureUrl { get; private set; } = string.Empty;
    public string CoverPictureUrl { get; private set; } = string.Empty;
    public SocialLinks? SocialLinks { get; private set; }
}
