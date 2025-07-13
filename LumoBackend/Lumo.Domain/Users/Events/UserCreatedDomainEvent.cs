using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
