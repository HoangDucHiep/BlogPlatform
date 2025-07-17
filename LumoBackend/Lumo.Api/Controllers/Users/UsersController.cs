using Lumo.Application.Users.LoginUser;
using Lumo.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Controllers.Users;


[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly HttpClient _httpClient;

    public UsersController(ISender sender, IHttpClientFactory httpClientFactory)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _httpClient = httpClientFactory?.CreateClient() 
            ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Username,
            request.Password);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(
            request.Email,
            request.Password);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    
}
