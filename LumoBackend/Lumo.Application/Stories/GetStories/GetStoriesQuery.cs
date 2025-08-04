using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Stories;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Application.Stories.GetStories;

/// <summary>
/// Query to get a paginated list of stories with optional filtering and sorting.
/// </summary>
public record GetStoriesQuery : IQuery<PaginationResult<StoryDto>>, IPageableRequest, ISortableRequest, ISearchRequest
{

    /* -- Filtering -- */
    /// <summary>
    /// Filter stories by author ID.
    /// </summary>
    [FromQuery(Name = "authorId")]
    public Guid? AuthorId { get; init; }

    /// <summary>
    /// Filter stories by status (Draft, Published, etc.).
    /// </summary>
    [FromQuery(Name = "status")]
    public StoryStatus? Status { get; init; }

    /// <summary>
    /// Filter stories by publication ID.
    /// </summary>
    [FromQuery(Name = "publicationId")]
    public Guid? PublicationId { get; init; }

    /// <summary>
    /// Filter stories by paywall status.
    /// </summary>
    [FromQuery(Name = "isPaywalled")]
    public bool? IsPaywalled { get; init; }


    /* -- Pagination -- */
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


    // Change the property type to nullable string to match ISortableRequest.Sort
    [FromQuery(Name = "sort")]
    public string Sort { get; init; } = "createdAtUtc DESC";

    /* -- Searching -- */
    [FromQuery(Name = "q")]
    public string? SearchQuery { get; init; }
}
