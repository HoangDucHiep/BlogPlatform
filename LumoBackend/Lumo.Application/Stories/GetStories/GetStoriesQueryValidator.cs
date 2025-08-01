using System.Text.RegularExpressions;
using FluentValidation;

namespace Lumo.Application.Stories.GetStories;
public class GetStoriesQueryValidator : AbstractValidator<GetStoriesQuery>
{
    private static readonly Regex SortFormatRegex = new(
        @"^([a-zA-Z0-9_.]+)(?:\s+(asc|desc))?(?:,([a-zA-Z0-9_.]+)(?:\s+(asc|desc))?)*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public GetStoriesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be greater than or equal to 1.");

        When(x => !string.IsNullOrWhiteSpace(x.Sort), () =>
        {
            RuleFor(x => x.Sort)
                .Must(BeValidSortFormat)
                .WithMessage("Invalid sort format. Format should be: 'property [asc|desc],property2 [asc|desc]'.");
        });
    }

    private bool BeValidSortFormat(string sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }

        return SortFormatRegex.IsMatch(sort);
    }
}
