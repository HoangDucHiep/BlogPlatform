using Lumo.Application.Users.GetLoggedInUser;
using Lumo.Application.Users.LoginUser;
using Lumo.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Controllers.Users;


[ApiController]
[Authorize]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly HttpClient httpClient;

    public UsersController(ISender sender, IHttpClientFactory httpClientFactory)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        httpClient = httpClientFactory?.CreateClient() 
            ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    [AllowAnonymous]
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

    [HttpGet("me")]
    public async Task<IActionResult> GetLoggedInUser(
        CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("test-info")]
    public async Task<IActionResult> GetTestInfo(
        CancellationToken cancellationToken)
    {
        string url = "http://idprovider:8080/realms/LumoBlog/protocol/openid-connect/userinfo";
        string accessToken = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI1Z3F1ZUJjelpCM1p3ZzcxbmNpQUc2c0ZyMm5RcU9oc2R1YzNFaUctdHI4In0.eyJleHAiOjE3NTI4MjkyMzgsImlhdCI6MTc1MjgyODkzOCwianRpIjoib2ZydHJvOmNkZDhiNDQ3LWM4YWEtMzVkZC0xOTJmLWM5ZGFmNDlkMTg4MSIsImlzcyI6Imh0dHA6Ly9pZHByb3ZpZGVyOjgwODAvcmVhbG1zL0x1bW9CbG9nIiwic3ViIjoiM2RiNGVmM2ItYzRlNy00OTFkLTg3YTgtYTk0Y2ZlMTBlMGI0IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoibHVtb2Jsb2ctYXV0aC1jbGllbnQiLCJzaWQiOiIyY2I5NDA5Mi1jYTU4LTRmYjUtYWE3OC0yOGMzZjMwYWNmMmIiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHA6Ly9sb2NhbGhvc3Q6ODA4MCJdLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsiZGVmYXVsdC1yb2xlcy1sdW1vYmxvZyIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwgb2ZmbGluZV9hY2Nlc3MiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwicHJlZmVycmVkX3VzZXJuYW1lIjoidXNlcnRlc3QxMkBleGFtcGxlLmNvbSIsImVtYWlsIjoidXNlcnRlc3QxMkBleGFtcGxlLmNvbSJ9.KBXvj3-wfBZdglKm0KN6o6UH0WRNMfSzwiVVSk99m_slKgiagKd-3kc2D5HxyY3UBnoKw20IvkSePdIA7MXelW7l4AGBP4TywBe7t5gOMnwHS8rGLb5-2lrGGMDIWZimLKAduFLFzbVf06nhSAFLzblRZB0aZYyEkrbdxDAfcnEoh9bgaIfyhARv0Yse7RL8o9lt4LAHs5f3v2EI5Ig2p5ObIwX9qqE0yxzYFtzwqFQip0bPhTqtrZ3vLJLpuVRPUb67DoNvIumCLyKlQ3i8wtS8GaxHXwo5EeY9iPNy1tB5TbGWJHHOL6zu5s7jd_1Mmhq_LO7XEy45pGF12oVvbg";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Failed to retrieve test info from Keycloak.");
        }   

        var userInfo = await response.Content.ReadFromJsonAsync<object>(cancellationToken);
        if (userInfo is null)
        {
            return NotFound("User info not found.");
        }

        return Ok(userInfo);
    }
}
