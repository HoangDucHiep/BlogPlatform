using Lumo.Api.Extensions;
using Lumo.Application.Stories.GetStories;
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
    public async Task<IActionResult> GetPublicStories(GetStoriesQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblemDetails(result);
        }

        return Ok(result.Value);
    }
}
