namespace Lumo.Application.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNowOffset { get; }
    DateTimeOffset GetDateTimeOffsetUtc (DateTime dateTime);
}
