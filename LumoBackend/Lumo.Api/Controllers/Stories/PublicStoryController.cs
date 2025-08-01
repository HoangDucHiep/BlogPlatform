using Lumo.Api.Extensions;
using Lumo.Application.Stories.GetStories;
using Lumo.Domain.Stories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Controllers.Stories;
[Route("/api/v1/stories")]
[ApiController]
public class PublicStoryController : ControllerBase
{
    private readonly ISender _sender;

    public PublicStoryController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    [HttpGet]
    public async Task<IActionResult> GetPublicStories(CancellationToken cancellationToken = default)
    {
        var query = new GetStoriesQuery
        {
            Status = StoryStatus.Published,
            Sort = "title desc"
        };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblemDetails(result);
        }

        return Ok(result.Value);
    }
}
