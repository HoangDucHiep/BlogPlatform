using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Stories;
public static class StoryError
{
    public static readonly Error NotFound = new(
        "Story.Found",
        "The story with the specified identifier was not found");
    public static readonly Error InvalidInput = new(
        "Story.InvalidInput",
        "The provided input for the story is invalid. Please ensure all required fields are filled out correctly."
    );
}
