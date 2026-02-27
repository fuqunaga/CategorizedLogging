using UnityEngine;

namespace CategorizedLogging
{
    public interface ISink
    {
        /// <summary>
        /// Add a log entry
        /// must be thread-safe
        /// </summary>
        [HideInCallstack]
        void Log(in LogEntry logEntry);
    }
}