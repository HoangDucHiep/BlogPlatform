using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;

namespace Lumo.Application.Stories.CreateNewDraft;
public sealed record CreateNewDraftCommand(
    string? Title,
    string? Content
) : ICommand<StoryDto>;
