using System;
using JetBrains.Annotations;

namespace ScotchLog
{
    public static partial class Log
    {
        /// <summary>
        /// Starts listening to log messages on the current thread with the specified log level filter.
        /// </summary>
        [MustDisposeResource]
        public static AsyncLocalSinkScope Listen(LogLevel logLevel, Action<string> logMessageCallback)
        {
            return new AsyncLocalSinkScope(new ListenerSink(logMessageCallback), logLevel);
        }

        /// <summary>
        /// Starts listening to log messages on the current thread with the specified log level filter.
        /// </summary>
        [MustDisposeResource]
        public static AsyncLocalSinkScope Listen(LogLevel logLevel, Action<LogEntry> logCallback)
        {
            return new AsyncLocalSinkScope(new ListenerSink(logCallback), logLevel);
        }
        
        /// <summary>
        /// Starts listening to log messages on the current thread with the specified log level filter.
        /// </summary>
        [MustDisposeResource]
        public static AsyncLocalSinkScope Listen(LogFilter filter, Action<string> logMessageCallback)
        {
            return new AsyncLocalSinkScope(new ListenerSink(logMessageCallback), filter);
        }

        /// <summary>
        /// Starts listening to log messages on the current thread with the specified log level filter.
        /// </summary>
        [MustDisposeResource]
        public static AsyncLocalSinkScope Listen(LogFilter filter,  Action<LogEntry> logCallback)
        {
            return new AsyncLocalSinkScope(new ListenerSink(logCallback), filter);
        }
    }
}