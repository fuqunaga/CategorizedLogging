using System.Collections.Concurrent;
using UnityEngine;

namespace CategorizedLogging
{
    public class LogToUnityDebugLog : ILogger
    {
        public ConcurrentDictionary<LogLevel, LogType?> LogLevelToUnityLogTypeTable { get; } = new()
        {
            [LogLevel.Trace] = null,
            [LogLevel.Debug] = LogType.Log,
            [LogLevel.Information] = LogType.Log,
            [LogLevel.Warning] = LogType.Warning,
            [LogLevel.Error] = LogType.Error,
            [LogLevel.Critical] = LogType.Error,
            [LogLevel.None] = null,
        };

        
        public void Log(in LogEntry logEntry)
        {
            if (LogLevelToUnityLogTypeTable.TryGetValue(logEntry.LogLevel, out var unityLogType)
                && unityLogType is { } logType
               )
            {
                Debug.unityLogger.Log(logType, logEntry.ToString());
            }
        }
    }
}