using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Users;

/// <summary>
/// This class contains static instances of common user-related errors.
/// These errors can be used throughout the application to handle user-related exceptions consistently.
/// </summary>
public static class UserErrors
{
    /// <summary>
    /// Represents an error when a user is not found.
    /// This error is used when a user with the specified identifier does not exist in the system.
    /// </summary>
    public static readonly Error NotFound = new(
        "User.Found",
        "The user with the specified identifier was not found");

    /// <summary>
    /// Represents an error when provided credentials are invalid.
    /// This error is used when the user attempts to log in with incorrect credentials.
    /// </summary>
    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid");



}
