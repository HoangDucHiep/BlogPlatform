using System.Text.RegularExpressions;

namespace Lumo.Domain.Users;

public record EmailAddress(string Value)
{
    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("EmailAddress address cannot be null or empty.", nameof(value));
        }
        if (!Regex.IsMatch(value, @"^\S+@\S+\.\S+$"))
        {
            throw new ArgumentException("Invalid email address format.", nameof(value));
        }
        return new EmailAddress(value);
    }
    public override string ToString() => Value;
}
