using System.Threading.Tasks;
using Lumo.Application.Stories.CreateNewDraft;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Controllers.Stories;

[ApiController]
[Authorize]
[Route("api/v1/stories/draft")]
public class DraftController : ControllerBase
{
    private readonly ISender _sender;

    public DraftController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDraft([FromBody] CreateNewDraftRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Title == null || request.Content == null)
        {
            return BadRequest("Title and Content cannot be null.");
        }

        var command = new CreateNewDraftCommand(
            request.Title,
            request.Content
        );

        // Assuming you need to send the command and handle the result
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(CreateDraft), new { id = result.Value.Id }, result.Value);
    }
}
