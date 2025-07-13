using System.Text.RegularExpressions;

namespace Lumo.Domain.Users;

public record Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Username is required");
        }

        if (value.Length < 3 || value.Length > 20)
        {
            throw new ArgumentException("Username must be between 3 and 20 characters");
        }

        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
        {
            throw new ArgumentException("Username can only contain letters, numbers, and underscores");
        }

        return new Name(value);
    }

    public override string ToString() => Value;
}
