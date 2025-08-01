using System.Diagnostics;
using Lumo.Application.Exceptions;
using Lumo.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Api.Helpers;

public static class ProblemDetailsFactory
{
    /// <summary>
    /// Creates a RFC 7807 problem details from a Result object
    /// </summary>
    public static ProblemDetails CreateFromResult(Result result, string? instance = null, string? traceId = null)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create problem details for a successful result");
        }

        var problemDetails = new ProblemDetails
        {
            Type = GetProblemTypeUri(result.Error!.Code),
            Title = GetProblemTitle(result.Error.Code),
            Status = GetStatusCode(result.Error.Code),
            Detail = result.Error.Message,
            Instance = instance
        };

        AddStandardExtensions(problemDetails, traceId);
        problemDetails.Extensions["errorCode"] = result.Error.Code;

        return problemDetails;
    }

    /// <summary>
    /// Creates a RFC 7807 problem details from an Exception
    /// </summary>
    public static ProblemDetails CreateFromException(
        Exception exception,
        string? instance = null,
        string? traceId = null,
        IEnumerable<object>? validationErrors = null)
    {
        var (type, title, status, detail) = GetExceptionInfo(exception);

        var problemDetails = new ProblemDetails
        {
            Type = GetProblemTypeUri(type),
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance
        };

        AddStandardExtensions(problemDetails, traceId);

        if (validationErrors != null)
        {
            problemDetails.Extensions["errors"] = validationErrors;
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a RFC 7807 problem details for 401 Unauthorized responses
    /// </summary>
    public static ProblemDetails CreateForUnauthorized(
        string detail,
        string? instance = null,
        string? traceId = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = detail,
            Instance = instance
        };

        AddStandardExtensions(problemDetails, traceId);

        return problemDetails;
    }

    private static void AddStandardExtensions(ProblemDetails problemDetails, string? traceId = null)
    {
        // Add standard extensions for tracking and debugging
        problemDetails.Extensions["traceId"] = traceId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString();
        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    }

    private static string GetProblemTypeUri(string type)
    {
        var baseUri = "https://lumo.api/problems/";

        // Normalize type name by removing domain prefix if present
        var normalizedType = type.Contains('.')
            ? type[(type.LastIndexOf('.') + 1)..].ToLower()
            : type.ToLower();

        return normalizedType switch
        {
            // Authentication/Authorization
            "notfound" => $"{baseUri}not-found",
            "invalidcredentials" => $"{baseUri}invalid-credentials",
            "unauthorized" => "https://tools.ietf.org/html/rfc7235#section-3.1",
            "forbidden" => "https://tools.ietf.org/html/rfc7231#section-6.5.3",

            // Validation
            "nullvalue" => $"{baseUri}null-value",
            "validation-error" => $"{baseUri}validation-error",
            "invalid-operation" => $"{baseUri}invalid-operation",
            "invalidinput" => $"{baseUri}invalid-input",

            // Domain-specific
            "authentication-failed" => "https://tools.ietf.org/html/rfc7235#section-3.1",

            _ => "about:blank"
        };
    }

    private static string GetProblemTitle(string errorCode)
    {
        return errorCode switch
        {
            "User.NotFound" => "User Not Found",
            "User.InvalidCredentials" => "Invalid Credentials",
            "Story.NotFound" => "Story Not Found",
            "Story.InvalidInput" => "Title and Content cannot be null or empty",
            "Error.NullValue" => "Null Value",
            _ => "An Error Occurred"
        };
    }

    private static int GetStatusCode(string errorCode)
    {
        // Extract error category from the error code
        var category = errorCode.Contains('.')
            ? errorCode.Split('.')[1].ToLowerInvariant()
            : errorCode.ToLowerInvariant();

        return category switch
        {
            "notfound" => StatusCodes.Status404NotFound,
            "invalidcredentials" => StatusCodes.Status401Unauthorized,
            "unauthorized" => StatusCodes.Status401Unauthorized,
            "forbidden" => StatusCodes.Status403Forbidden,
            "nullvalue" => StatusCodes.Status400BadRequest,
            "invalid" => StatusCodes.Status400BadRequest,
            "validation" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static (string type, string title, int status, string detail) GetExceptionInfo(Exception ex)
    {
        return ex switch
        {
            ValidationException validationEx =>
                ("validation-error", "Validation Failed", StatusCodes.Status400BadRequest,
                "One or more validation errors occurred"),

            InvalidOperationException invalidOp =>
                ("invalid-operation", "Invalid Operation", StatusCodes.Status400BadRequest,
                invalidOp.Message),

            HttpRequestException httpEx when httpEx.Message.Contains("401") =>
                ("authentication-failed", "Authentication Failed", StatusCodes.Status401Unauthorized,
                "The provided credentials are invalid"),

            _ => ("internal-server-error", "Internal Server Error", StatusCodes.Status500InternalServerError,
                "An unexpected error occurred")
        };
    }
}
