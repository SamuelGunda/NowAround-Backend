using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NowAround.Application.Common.Exceptions;

namespace NowAround.WebApi.Middleware;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _exceptionHandler;

    public GlobalExceptionHandlerTests()
    {
        _exceptionHandler = new GlobalExceptionHandler();
    }

    [Theory]
    [InlineData(typeof(EmailAlreadyInUseException), StatusCodes.Status409Conflict, "Email already in use")]
    [InlineData(typeof(EntityNotFoundException), StatusCodes.Status404NotFound, "Not found")]
    [InlineData(typeof(InvalidSearchActionException), StatusCodes.Status400BadRequest, "Invalid search action")]
    [InlineData(typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized, "Unauthorized")]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError, "Internal server error")]
    public async Task TryHandleAsync_ReturnsExpectedProblemDetails_ForDifferentExceptions(
        Type exceptionType, int expectedStatusCode, string expectedTitle)
    {
        // Arrange
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedStatusCode, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);

        Assert.NotNull(problemDetails);
        Assert.Equal(expectedStatusCode, problemDetails!.Status);
        Assert.Equal(expectedTitle, problemDetails.Title);
    }
    
    [Fact]
    public async Task TryHandleAsync_ReturnsExpectedProblemDetails_ForEntityAlreadyExistsException()
    {
        // Arrange
        var exception = new EntityAlreadyExistsException("User", "Email", "test@example.com");
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);

        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status409Conflict, problemDetails!.Status);
        Assert.Equal("Entity already exists", problemDetails.Title);
        Assert.Equal(exception.Message, problemDetails.Detail);
    }
}