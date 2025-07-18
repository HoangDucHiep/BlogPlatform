using Lumo.Application.Abstractions.Clock;

namespace Lumo.Infrastructure.Clock;

public sealed class DatetimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    public DateTimeOffset GetDateTimeOffsetUtc(DateTime dateTime)
    {
        throw new NotImplementedException();
    }
}
