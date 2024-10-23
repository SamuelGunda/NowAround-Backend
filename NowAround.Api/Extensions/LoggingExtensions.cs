﻿namespace NowAround.Api.Extensions;

public static class LoggingExtensions
{
    public static void AddLogging(this ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
        logging.SetMinimumLevel(LogLevel.Debug);
    }
}