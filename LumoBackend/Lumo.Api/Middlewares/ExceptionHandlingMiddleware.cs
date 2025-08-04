using Lumo.Api.Helpers;
using Lumo.Application.Exceptions;

namespace Lumo.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
            {
                var detail = DetermineUnauthorizedReason(context);
                var problemDetails = ProblemDetailsFactory.CreateForUnauthorized(
                    detail,
                    context.Request.Path.Value,
                    context.TraceIdentifier);

                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
        catch (SortFieldException ex)
        {
            _logger.LogError(ex, "Sort field exception occurred: {Message}", ex.Message);
            var problemDetails = ProblemDetailsFactory.CreateFromException(
                ex,
                context.Request.Path.Value,
                context.TraceIdentifier);
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            // Get validation errors if available
            IEnumerable<object>? validationErrors = null;
            if (ex is ValidationException validationEx)
            {
                validationErrors = validationEx.Errors;
            }

            // Create problem details from the exception
            var problemDetails = ProblemDetailsFactory.CreateFromException(
                ex,
                context.Request.Path.Value,
                context.TraceIdentifier,
                validationErrors);

            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static string DetermineUnauthorizedReason(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader))
        {
            return "No authorization token provided. Please include a valid Bearer token in the Authorization header.";
        }

        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid authorization header format. Expected 'Bearer <token>'.";
        }

        return "The provided authorization token is invalid or expired. Please obtain a new token.";
    }
}

public record ExceptionDetails(
    int Status,
    string Type,
    string Title,
    string Detail,
    IEnumerable<object>? Errors);
