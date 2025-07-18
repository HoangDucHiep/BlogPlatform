using Lumo.Application.Abstractions.Email;

namespace Lumo.Infrastructure.Email;

/// <summary>
/// Service for sending emails.
/// </summary>
public sealed class EmailService : IEmailService
{
    /// <summary>
    /// Sends an email to the specified recipient with the given subject and body.
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public Task SendAsync(Domain.Users.EmailAddress recipient, string subject, string body)
    {
        return Task.CompletedTask; // Placeholder for actual email sending logic 
    }

}
