namespace Lumo.Application.Stories.Dtos;

public record SaveChangeVersionDto
{
    public Guid Id { get; private set; }
    public Guid StoryId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; init; }

}
