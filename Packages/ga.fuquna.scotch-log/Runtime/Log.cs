using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace ScotchLog
{
    /// <summary>
    /// ScorpionLog インターフェース
    /// 
    /// カテゴリとして呼び出し元の型を使用する
    /// </summary>
    public static partial class Log
    {
        private static readonly AsyncLocal<ILogDispatcher> LogDispatcherAsyncLocal = new();
        
        
        public static ILogDispatcher LogDispatcher { get; set; } = new LogDispatcher();
        
        public static ILogDispatcher AsyncLocalLogDispatcher
        {
            get => LogDispatcherAsyncLocal.Value;
            set => LogDispatcherAsyncLocal.Value = value;
        }


        [HideInCallstack]
        private static void EmitLog(LogEntry logEntry)
        {
            LogDispatcher?.Log(logEntry);
            AsyncLocalLogDispatcher?.Log(logEntry);
            logEntry.Dispose();
        }

  
        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void EmitLog(LogLevel logLevel, in StringWrapper message, 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
            )
        {
            EmitLog(LogEntry.Rent(logLevel, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }


        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Trace(in StringWrapper message,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
        )
        {
            EmitLog(LogEntry.Rent(LogLevel.Trace, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }

        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Debug(in StringWrapper message,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
        )
        {
            EmitLog(LogEntry.Rent(LogLevel.Debug, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }
        
        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Information(in StringWrapper message,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
        )
        {
            EmitLog(LogEntry.Rent(LogLevel.Information, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }

        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Warning(in StringWrapper message,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
        )
        {
            EmitLog(LogEntry.Rent(LogLevel.Warning, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }

        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Error(in StringWrapper message,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
        )
        {
            EmitLog(LogEntry.Rent(LogLevel.Error, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }

        [HideInCallstack]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Fatal(in StringWrapper message,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = ""
        )
        {
            EmitLog(LogEntry.Rent(LogLevel.Fatal, message,
                new CallerInformation(callerFilePath, callerLineNumber, callerMemberName)));
        }
    }
}