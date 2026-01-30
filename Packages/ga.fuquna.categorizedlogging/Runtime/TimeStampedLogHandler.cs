using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CategorizedLogging
{
    /// <summary>
    /// Unityのログにタイムスタンプを追加する
    /// </summary>
    public class TimeStampedLogHandler : ILogHandler
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            // Player.logの出力にタイムスタンプを追加する
            Debug.unityLogger.logHandler = new TimeStampedLogHandler();
        }
        
        
        private readonly ILogHandler _defaultHandler = Debug.unityLogger.logHandler;

        
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            var timeStampedFormat = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{logType}]{format}";
            _defaultHandler.LogFormat(logType, context, timeStampedFormat, args);
        }

        public void LogException(Exception exception, Object context)
        {
            _defaultHandler.LogException(exception, context);
        }
    }
}