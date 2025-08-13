using Lumo.Api.Dtos.Requests;
using Lumo.Api.Extensions;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Stories.CreateNewDraft;
using Lumo.Application.Stories.GetStories;
using Lumo.Domain.Stories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Controllers.Stories;

[ApiController]
[Authorize]
[Route("/api/v1/users/me/stories")]
public class UserStoryController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;

    public UserStoryController(ISender sender, IUserContext userContext)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    [HttpPost("draft")]
    public async Task<IActionResult> CreateDraft([FromBody] CreateNewDraftRequest request, CancellationToken cancellationToken = default)
    {

        var command = new CreateNewDraftCommand(
            request.Title,
            request.Content
        );

        // Assuming you need to send the command and handle the result
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblemDetails(result);
        }

        return CreatedAtAction(nameof(CreateDraft), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserStories(CancellationToken cancellationToken = default)
    {
        var query = new GetStoriesQuery
        {
            AuthorId = await _userContext.UserId()
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblemDetails(result);
        }

        return Ok(result.Value);
    }
}

public sealed record StoryQueryParameter
{
    [FromQuery(Name = "status")]
    public StoryStatus? Status { get; set; }
}
