using Microsoft.Azure.WebJobs.Host;

namespace KokeiroLifeLogger.Common
{
    public static class AzureFunctionLogger
    {
        public static TraceWriter Logger { get; set; }

        public static void Error(string message)
        {
            Logger?.Error(message);
        }

        public static void Information(string message)
        {
            Logger?.Info(message);
        }

        public static void Warning(string message)
        {
            Logger?.Warning(message);
        }
    }
}
