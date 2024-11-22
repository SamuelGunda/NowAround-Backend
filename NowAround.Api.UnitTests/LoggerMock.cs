using Microsoft.Extensions.Logging;
using Moq;

namespace NowAround.Api.UnitTests;

public class LogMessage
{
    public LogLevel LogLevel { get; set; }
    public string Message { get; set; }
    public Exception Exception { get; set; }
}

public class LoggerMock<T> : Mock<ILogger<T>>
{
    private readonly List<LogMessage> _loggedMessages = new();

    public LoggerMock()
    {
        Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
            .Callback<LogLevel, EventId, object, Exception, Delegate>((logLevel, eventId, state, exception, formatter) =>
            {
                var message = formatter?.DynamicInvoke(state, exception) as string;
                _loggedMessages.Add(new LogMessage
                {
                    LogLevel = logLevel,
                    Message = message,
                    Exception = exception
                });
            });
    }

    public List<LogMessage> LoggedMessages => _loggedMessages;
    
    public void VerifyLog(LogLevel logLevel, Exception exception)
    {
        var logMessage = _loggedMessages.FirstOrDefault(log => log.LogLevel == logLevel && log.Exception == exception);
        if (logMessage == null)
        {
            throw new Exception($"Expected log with level {logLevel} and exception {exception} was not found.");
        }
    }
}