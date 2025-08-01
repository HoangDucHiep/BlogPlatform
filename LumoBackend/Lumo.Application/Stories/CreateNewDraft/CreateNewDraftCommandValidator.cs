using FluentValidation;

namespace Lumo.Application.Stories.CreateNewDraft;

public class CreateNewDraftCommandValidator : AbstractValidator<CreateNewDraftCommand>
{
    public CreateNewDraftCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull()
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Content)
            .NotNull()
            .NotEmpty().WithMessage("Content is required");
    }
}
