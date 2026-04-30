using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace ScotchLog
{
    public class UnityLoggerSink : ISink
    {
        public Func<LogEntry, string> LogEntryToMessageFormatter { get; set; } = logRecord => logRecord.ToString();
        
        public ConcurrentDictionary<LogLevel, LogType?> LogLevelToUnityLogTypeTable { get; } = new()
        {
            [LogLevel.Trace] = LogType.Log,
            [LogLevel.Debug] = LogType.Log,
            [LogLevel.Information] = LogType.Log,
            [LogLevel.Warning] = LogType.Warning,
            [LogLevel.Error] = LogType.Error,
            [LogLevel.Fatal] = LogType.Error,
            [LogLevel.None] = null,
        };

        
        [HideInCallstack]
        public void Log(LogEntry logEntry)
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