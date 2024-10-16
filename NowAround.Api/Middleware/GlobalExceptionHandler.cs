using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Exceptions;

namespace NowAround.Api.Middleware;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        var problemDetails = exception switch
        {
            EmailAlreadyInUseException emailAlreadyInUseException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Email already in use",
                Detail = emailAlreadyInUseException.Message
            },
            EstablishmentAlreadyExistsException establishmentAlreadyExistsException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Establishment already exists",
                Detail = establishmentAlreadyExistsException.Message
            },
            EstablishmentNotFoundException establishmentNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Establishment not found",
                Detail = establishmentNotFoundException.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred. Please try again later."
            }
        };

        if (problemDetails.Status != null)
        {
            httpContext.Response.StatusCode = problemDetails.Status.Value;
        }

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}