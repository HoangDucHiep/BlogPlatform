namespace Lumo.Domain.Abstractions;

/// <summary>
/// Record representing an error with a code and name.
/// </summary>
/// <param name="Code"></param>
/// <param name="Name"></param>
public record Error(string Code, string Name)
{
    public static Error None = new(string.Empty, string.Empty);
    public static Error NullValue = new("Error.NullValue", "Null value was provided");
}
