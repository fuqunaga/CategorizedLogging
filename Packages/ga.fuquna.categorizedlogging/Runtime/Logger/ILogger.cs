namespace CategorizedLogging
{
    public interface ILogger
    {
        /// <summary>
        /// Add a log entry
        /// must be thread-safe
        /// </summary>
        void Log(in LogEntry logEntry);
    }
}