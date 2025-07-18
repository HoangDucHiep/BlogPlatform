namespace Lumo.Domain.Utils;

public static class IdGenerator
{
    public static Guid GenerateId() => Guid.CreateVersion7();
}
