using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;

namespace KokeiroLifeLogger.Utilities
{
    static class TraceWriterExt
    {
        public static void Exception(this TraceWriter log, Exception ex)
        {
            log.Error(ex.Message, ex);
        }
    }
}
