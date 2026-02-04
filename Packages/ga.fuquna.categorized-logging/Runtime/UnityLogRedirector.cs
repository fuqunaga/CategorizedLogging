using System.Collections.Concurrent;
using UnityEngine;

namespace CategorizedLogging
{
    /// <summary>
    /// Debug.Log()などの呼び出しをCategorizedLoggingのLog.*()に転送する
    /// 
    /// Known Issue:
    /// - UnityLoggerSinkと併用しするとコンソール出力がUnityのものとCategorizedLogging経由のもので２重に表示されてしまう
    /// </summary>
    public static class UnityLogRedirector
    {
        private static bool _enabled;
        
        public static bool Enabled { 
            get => _enabled;
            set
            {
                if (value == _enabled)
                {
                    return;
                }
                
                _enabled = value;
                if (_enabled)
                {
                    Application.logMessageReceived += HandleLog;
                }
                else
                {
                    Application.logMessageReceived -= HandleLog;
                }
            }
            
        }
        
        public static string Category { get; set; } = "Unity";
        
        public static ConcurrentDictionary<LogType, LogLevel> UnityLogTypeToLogLevelTable { get; } = new()
        {
            [LogType.Error] = LogLevel.Error,
            [LogType.Assert] = LogLevel.Error,
            [LogType.Warning] = LogLevel.Warning,
            [LogType.Log] = LogLevel.Debug,
            [LogType.Exception] = LogLevel.Error,
        };
        
        private static void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (UnityLogTypeToLogLevelTable.TryGetValue(type, out var logLevel) && logLevel != LogLevel.None)
            {
                Log.EmitLog(Category, logLevel, condition);
            }
        }
    }
}