using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Stories;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Application.Stories.GetStories;

/// <summary>
/// Query to get a paginated list of stories with optional filtering and sorting.
/// </summary>
public record GetStoriesQuery : IQuery<PaginationResult<StoryDto>>, IPageableRequest, ISortableRequest
{
    /// <summary>
    /// Filter stories by author ID.
    /// </summary>
    public Guid? AuthorId { get; init; }

    /// <summary>
    /// Filter stories by status (Draft, Published, etc.).
    /// </summary>
    public StoryStatus? Status { get; init; }

    /// <summary>
    /// Filter stories by publication ID.
    /// </summary>
    public Guid? PublicationId { get; init; }

    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    [FromQuery(Name = "page")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page. Default is 10.
    /// </summary>
    [FromQuery(Name = "pageSize")]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Sorting specification in format: property [asc|desc],property2 [asc|desc]
    /// Example: sort=title desc,publishedAtUtc,authorId
    /// </summary>
    [FromQuery(Name = "sort")]
    public string Sort { get; init; } = string.Empty;
}
