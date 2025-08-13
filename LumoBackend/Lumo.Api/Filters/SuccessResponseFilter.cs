using Lumo.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lumo.Api.Filters;

public class SuccessResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Do nothing on action executing
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Chỉ wrap các response CÓ BODY (có data)
        switch (context.Result)
        {
            // 200 OK with data
            case OkObjectResult okResult when okResult.Value != null:
                context.Result = new OkObjectResult(SuccessfullyResponse.Create(okResult.Value));
                break;

            // 201 Created with data
            case CreatedResult createdResult when createdResult.Value != null:
                context.Result = new CreatedResult(createdResult.Location,
                    SuccessfullyResponse.Create(createdResult.Value));
                break;

            // 201 Created at action with data
            case CreatedAtActionResult createdAtActionResult when createdAtActionResult.Value != null:
                context.Result = new CreatedAtActionResult(
                    createdAtActionResult.ActionName,
                    createdAtActionResult.ControllerName,
                    createdAtActionResult.RouteValues,
                    SuccessfullyResponse.Create(createdAtActionResult.Value));
                break;

            // 201 Created at route with data
            case CreatedAtRouteResult createdAtRouteResult when createdAtRouteResult.Value != null:
                context.Result = new CreatedAtRouteResult(
                    createdAtRouteResult.RouteName,
                    createdAtRouteResult.RouteValues,
                    SuccessfullyResponse.Create(createdAtRouteResult.Value));
                break;

            // 202 Accepted with data
            case AcceptedResult acceptedResult when acceptedResult.Value != null:
                context.Result = new AcceptedResult(acceptedResult.Location,
                    SuccessfullyResponse.Create(acceptedResult.Value));
                break;

            // 202 Accepted at action with data
            case AcceptedAtActionResult acceptedAtActionResult when acceptedAtActionResult.Value != null:
                context.Result = new AcceptedAtActionResult(
                    acceptedAtActionResult.ActionName,
                    acceptedAtActionResult.ControllerName,
                    acceptedAtActionResult.RouteValues,
                    SuccessfullyResponse.Create(acceptedAtActionResult.Value));
                break;

            // 202 Accepted at route with data
            case AcceptedAtRouteResult acceptedAtRouteResult when acceptedAtRouteResult.Value != null:
                context.Result = new AcceptedAtRouteResult(
                    acceptedAtRouteResult.RouteName,
                    acceptedAtRouteResult.RouteValues,
                    SuccessfullyResponse.Create(acceptedAtRouteResult.Value));
                break;

            // Bất kỳ ObjectResult nào khác có data và status 2xx
            case ObjectResult objectResult when objectResult.Value != null &&
                                                objectResult.StatusCode >= 200 &&
                                                objectResult.StatusCode < 300:
                context.Result = new ObjectResult(SuccessfullyResponse.Create(objectResult.Value))
                {
                    StatusCode = objectResult.StatusCode
                };
                break;

                // KHÔNG wrap các response KHÔNG có body:
                // - OkResult (200 OK no body)
                // - NoContentResult (204 No Content)
                // - StatusCodeResult (các status code không có data)
        }
    }
}
