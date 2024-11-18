﻿using Microsoft.AspNetCore.Diagnostics;
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
        
        Console.WriteLine($"Exception caught by GlobalExceptionHandler: {exception.Message}");

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
            EntityNotFoundException entityNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = entityNotFoundException.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
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