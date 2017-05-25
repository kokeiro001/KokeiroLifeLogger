using System;
using Microsoft.Azure.WebJobs.Host;

namespace KokeiroLifeLogger.Utilities
{
    static class TraceWriterExt
    {
        public static void Exception(this TraceWriter log, Exception ex)
        {
            log.Error(ex.Message + Environment.NewLine + ex.StackTrace, ex);
        }
    }
}
