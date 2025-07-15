namespace Lumo.Application.Exceptions;


/// <summary>
/// Represents an exception that occurs when a concurrency conflict is detected.
/// This exception is typically thrown when multiple operations attempt to modify the same resource concurrently,
/// </summary>
public sealed class ConcurrencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ConcurrencyException"/> class with a specified error message and an inner exception.
    /// The inner exception is used to provide additional context about the error that caused this exception.
    /// </remarks>
    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

}
