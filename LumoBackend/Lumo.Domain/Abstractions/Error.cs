namespace Lumo.Domain.Abstractions;

/// <summary>
/// Record representing an error with a code and name.
/// </summary>
/// <param name="Code"></param>
/// <param name="Message"></param>
public record Error(string Code, string Message)
{
    public static Error? None; // null
    public static Error NullValue = new("Error.NullValue", "Null value was provided");
}
