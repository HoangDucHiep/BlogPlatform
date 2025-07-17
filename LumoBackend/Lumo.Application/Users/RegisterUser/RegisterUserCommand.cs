using Lumo.Application.Abstractions.Messaging;

namespace Lumo.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string UserName,
    string Password
) : ICommand<Guid>;
