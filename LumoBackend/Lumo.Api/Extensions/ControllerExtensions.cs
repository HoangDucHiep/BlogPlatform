using Lumo.Api.Helpers;
using Lumo.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Extensions;

public static class ControllerExtensions
{
    // Sử dụng string category thay vì static class
    private static readonly Action<ILogger, string?, Exception?> _unexpectedSuccessResult =
        LoggerMessage.Define<string?>(
            LogLevel.Warning,
            new EventId(1, nameof(ToProblemDetails)),
            "ToProblemDetails called with success result for {Path}"); // Thêm semicolon


    private static ILogger GetLogger(ControllerBase controller)
    {
        // Sử dụng ILoggerFactory thay vì ILogger<T>
        var loggerFactory = controller.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>();
        return loggerFactory.CreateLogger("Lumo.Api.Extensions.ControllerExtensions");
    }

    // For Result (non-generic)
    public static IActionResult ToProblemDetails(this ControllerBase controller, Result result)
    {
        var logger = GetLogger(controller);

        if (result.IsSuccess)
        {
            _unexpectedSuccessResult(logger, controller.HttpContext.Request.Path.Value, null);
            return controller.Ok(); // No value to return
        }

        var problemDetails = ProblemDetailsFactory.CreateFromResult(
            result,
            controller.HttpContext.Request.Path.Value,
            controller.HttpContext.TraceIdentifier);

        return controller.StatusCode(problemDetails.Status!.Value, problemDetails);
    }

    // For Result<T> (generic)
    public static IActionResult ToProblemDetails<T>(this ControllerBase controller, Result<T> result)
    {
        var logger = GetLogger(controller);

        if (result.IsSuccess)
        {
            _unexpectedSuccessResult(logger, controller.HttpContext.Request.Path.Value, null);
            return controller.Ok(result.Value);
        }

        var problemDetails = ProblemDetailsFactory.CreateFromResult(
            result,
            controller.HttpContext.Request.Path.Value,
            controller.HttpContext.TraceIdentifier);

        return controller.StatusCode(problemDetails.Status!.Value, problemDetails);
    }

    private static string GetErrorTypeUri(string errorCode)
    {
        var baseUri = "https://lumo.api/problems/";

        return errorCode switch
        {
            // User errors
            "User.NotFound" => $"{baseUri}user-not-found",
            "User.InvalidCredentials" => $"{baseUri}invalid-credentials",

            // Story errors
            "Story.NotFound" => $"{baseUri}story-not-found",

            // Generic errors
            "Error.NullValue" => $"{baseUri}null-value",

            _ => "about:blank"
        };
    }

    private static string GetErrorTitle(string errorCode)
    {
        return errorCode switch
        {
            "User.NotFound" => "User Not Found",
            "User.InvalidCredentials" => "Invalid Credentials",
            "Story.NotFound" => "Story Not Found",
            "Error.NullValue" => "Null Value",
            _ => "An Error Occurred"
        };
    }

    private static int GetErrorStatusCode(string errorCode)
    {
        return errorCode switch
        {
            "User.NotFound" => 404,
            "User.InvalidCredentials" => 401,
            "Story.NotFound" => 404,
            "Error.NullValue" => 400,
            _ => 500
        };
    }
}
