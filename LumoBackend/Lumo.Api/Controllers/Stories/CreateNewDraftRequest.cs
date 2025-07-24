namespace Lumo.Api.Controllers.Stories;

public sealed record CreateNewDraftRequest
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}
