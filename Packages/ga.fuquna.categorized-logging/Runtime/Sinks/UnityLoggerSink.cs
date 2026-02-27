using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CategorizedLogging
{
    public class UnityLoggerSink : ISink
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

        
        [HideInCallstack]
        public void Log(in LogEntry logEntry)
        {
            if (LogLevelToUnityLogTypeTable.TryGetValue(logEntry.LogLevel, out var unityLogType)
                && unityLogType is { } logType
               )
            {
                Debug.unityLogger.Log(logType, LogEntryToMessageFormatter(logEntry));
            }
        }
        
        // 明示的なインターフェース実装に属性を付与してブリッジメソッドを上書きして
        // ブリッジメソッドも HideInCallstack にする
        [HideInCallstack]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ISink.Log(in LogEntry logEntry)
        {
            Log(logEntry);
        }
    }
}