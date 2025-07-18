using Lumo.Domain.Users;

namespace Lumo.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterUserAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);
}
