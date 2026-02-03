using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace CategorizedLogging
{
    public class CategorizedLogToUnityDebugLog : ILogger
    {
        public delegate string LogEntryToMessage(in LogEntry logEntry);
        
        public LogEntryToMessage LogEntryToMessageFormatter { get; set; } = (in LogEntry logEntry) => logEntry.ToString();
        
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
                Debug.unityLogger.Log(logType, LogEntryToMessageFormatter(logEntry));
            }
        }
    }
}