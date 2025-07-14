namespace Lumo.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendAsync(Domain.Users.EmailAddress recipient, string subject, string body);
}
