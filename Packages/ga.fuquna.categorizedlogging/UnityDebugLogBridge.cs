using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace CategorizedLogging
{
    /// <summary>
    /// Debug.Log系の呼び出しをCategorizedLoggingのLog.*()に転送する
    /// </summary>
    public static class UnityDebugLogBridge
    {
        public static ConcurrentDictionary<LogType, LogLevel> UnityLogTypeToLogLevelTable { get; } = new()
        {
            [LogType.Error] = LogLevel.Error,
            [LogType.Assert] = LogLevel.Error,
            [LogType.Warning] = LogLevel.Warning,
            [LogType.Log] = LogLevel.Debug,
            [LogType.Exception] = LogLevel.Error,
        };

        
        public static Func<string, string, string>  UncaughtExceptionLogToMessageFormatter { get; set; } = (condition, stackTrace) =>
            $"Uncaught Exception: {condition}\n{stackTrace}";
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            // catchされなかったExceptionのログをLoggerに渡す
            Application.logMessageReceivedThreaded += HandleLog;
        }

        private static void HandleLog(string condition, string stackTrace, LogType type)
        {
            // // catchされなかったExceptionのログをInGameLogに追加する
            // if (type == LogType.Exception)
            // {
            //     if (UncaughtExceptionLogToMessageFormatter is { } formatter)
            //     {
            //         Log.EmitLog(typeof(UnityDebugLogBridge), formatter.LogLevel,
            //             formatter.Format(condition, stackTrace));
            //     }
            // }
        }
    }
}