using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
