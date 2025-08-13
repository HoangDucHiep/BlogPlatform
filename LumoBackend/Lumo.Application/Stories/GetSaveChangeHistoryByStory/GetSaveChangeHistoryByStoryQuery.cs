using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Application.Stories.GetSaveChangeHistoryByStory;
public class GetSaveChangeHistoryByStoryQuery : IQuery<PaginationResult<SaveChangeVersionDto>>, IPageableRequest
{
    public Guid StoryId { get; init; }


    /* -- Pagination -- */
    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    [FromQuery(Name = "page")]
    public int Page { get; init; }

    /// <summary>
    /// Number of items per page. Default is 10.
    /// </summary>
    [FromQuery(Name = "pageSize")]
    public int PageSize { get; init; }
}
