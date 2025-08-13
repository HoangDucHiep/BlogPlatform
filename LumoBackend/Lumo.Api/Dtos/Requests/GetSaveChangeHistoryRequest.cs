using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Dtos.Requests;

public class GetSaveChangeHistoryRequest
{
    [FromRoute]
    public Guid StoryId { get; set; }

    [FromQuery]
    public int Page { get; set; } = 1;

    [FromQuery]
    public int PageSize { get; set; } = 10;
}
