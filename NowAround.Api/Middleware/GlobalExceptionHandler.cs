using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Authentication.Exceptions;

namespace NowAround.Api.Middleware;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case EmailAlreadyInUseException emailAlreadyInUseException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Email already in use";
                problemDetails.Detail = emailAlreadyInUseException.Message;
                break;
            
            case EstablishmentAlreadyExistsException establishmentAlreadyExistsException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Establishment already exists";
                problemDetails.Detail = establishmentAlreadyExistsException.Message;
                break;
            
            case EstablishmentNotFound establishmentNotFound:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Establishment not found";
                problemDetails.Detail = establishmentNotFound.Message;
                break;
            
            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}