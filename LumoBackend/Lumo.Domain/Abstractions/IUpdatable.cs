namespace Lumo.Domain.Abstractions;
public interface IUpdatable
{
    DateTimeOffset LastUpdatedAtUtc { get; set; }
}
