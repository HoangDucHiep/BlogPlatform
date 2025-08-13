namespace Lumo.Api.Dtos.Requests;

public sealed record CreateNewDraftRequest
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}
