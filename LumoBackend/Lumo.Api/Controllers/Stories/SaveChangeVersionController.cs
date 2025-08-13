using Lumo.Api.Dtos.Requests;
using Lumo.Api.Extensions;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Stories.GetSaveChangeHistoryByStory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Controllers.Stories;
[Route("api/v1")]
[ApiController]
public class SaveChangeVersionController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;

    public SaveChangeVersionController(ISender sender, IUserContext userContext)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    [Authorize]
    [HttpGet("stories/{storyId}/save-change-history")]
    public async Task<IActionResult> GetSaveChangeHistory(GetSaveChangeHistoryRequest request, CancellationToken cancellationToken)
    {
        var query = new GetSaveChangeHistoryByStoryQuery
        {
            StoryId = request.StoryId,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblemDetails(result);
        }
        return Ok(result.Value);

    }

}
