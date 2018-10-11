using Microsoft.Extensions.Logging;
using System;

namespace KokeiroLifeLogger.Utilities
{
    public static class LoggerExtensions
    {
        public static void LogException(this ILogger logger, Exception exception)
        {
            logger.LogError(new EventId(), exception.Message, exception);
        }
    }
}
