namespace Lumo.Api.Controllers.Users;


/// <summary>
/// Represents a request to register a new user.
/// </summary>
/// <param name="Email"></param>
/// <param name="Username"></param>
/// <param name="Password"></param>
public sealed record RegisterUserRequest(
    string Email,
    string Username,
    string Password
    );
