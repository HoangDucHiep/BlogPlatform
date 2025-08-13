using Lumo.Domain.Abstractions;
using Lumo.Domain.Users.Events;
using Lumo.Domain.Utils;

namespace Lumo.Domain.Users;

/// <summary>
/// Represents a user in the system.
/// /// This class contains properties related to the user's identity, profile, and social links.
/// </summary>
public class User : Entity, IUpdatable
{
    public string IdentityId { get; private set; }
    public Name UserName { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public string Bio { get; private set; } = string.Empty;
    public string ProfilePictureUrl { get; private set; } = string.Empty;
    public string CoverPictureUrl { get; private set; } = string.Empty;
    public SocialLinks? SocialLinks { get; private set; }
    public DateTimeOffset LastUpdatedAtUtc { get; set; }

    public User() { }

    public User(Guid id, Name userName, EmailAddress emailAddress)
        : base(id)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
    }

    public static User Create(Name userName, EmailAddress emailAddress)
    {
        var user = new User(IdGenerator.GenerateId(), userName, emailAddress);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void SetIdentityId(string identityId)
    {
        if (string.IsNullOrWhiteSpace(identityId))
        {
            throw new ArgumentException("Identity ID cannot be null or empty.", nameof(identityId));
        }
        IdentityId = identityId;
    }
}
