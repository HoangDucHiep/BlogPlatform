using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
