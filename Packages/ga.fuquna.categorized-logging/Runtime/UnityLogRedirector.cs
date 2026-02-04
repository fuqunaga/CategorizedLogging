using System.Collections.Concurrent;
using UnityEngine;

namespace CategorizedLogging
{
    /// <summary>
    /// Debug.Log系の呼び出しをCategorizedLoggingのLog.*()に転送する
    /// </summary>
    public static class UnityLogRedirector
    {
        public static ConcurrentDictionary<LogType, LogLevel> UnityLogTypeToLogLevelTable { get; } = new()
        {
            [LogType.Error] = LogLevel.Error,
            [LogType.Assert] = LogLevel.Error,
            [LogType.Warning] = LogLevel.Warning,
            [LogType.Log] = LogLevel.Debug,
            [LogType.Exception] = LogLevel.Error,
        };

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            // catchされなかったExceptionのログをLoggerに渡す
            Application.logMessageReceivedThreaded += HandleLog;
        }

        private static void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (UnityLogTypeToLogLevelTable.TryGetValue(type, out var logLevel) && logLevel != LogLevel.None)
            {
                Log.EmitLog("Unity", logLevel, condition);
            }
        }
    }
}