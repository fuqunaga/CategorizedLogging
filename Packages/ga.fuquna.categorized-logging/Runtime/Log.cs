using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace CategorizedLogging
{
    /// <summary>
    /// CategorizedLogging インターフェース
    /// 
    /// カテゴリとして呼び出し元の型を使用する
    /// </summary>
    public static class Log
    {
        public static LogDispatcher LogDispatcher { get; set; } = new();


        [HideInCallstack] public static void EmitLog(in LogEntry logEntry) => LogDispatcher?.Log(logEntry);
        [HideInCallstack] public static void EmitLog(string category, LogLevel logLevel, string message) => EmitLog(new LogEntry(logLevel, category, message));
        [HideInCallstack] public static void EmitLog(Type typeForCategory, LogLevel logLevel, string message) => EmitLog(typeForCategory.Name, logLevel, message);
        [HideInCallstack] public static void EmitLog<TCaller>(LogLevel logLevel, string message) => EmitLog(typeof(TCaller), logLevel, message);
        [HideInCallstack] public static void EmitLog<TCaller>(TCaller _, LogLevel logLevel, string message) => EmitLog<TCaller>(logLevel, message);
        
        
        [HideInCallstack] public static void Trace(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Trace, message);
        [HideInCallstack] public static void Debug(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Debug, message);
        [HideInCallstack] public static void Information(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Information, message);
        [HideInCallstack] public static void Warning(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Warning, message);
        [HideInCallstack] public static void Error(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Error, message);
        [HideInCallstack] public static void Critical(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Critical, message);
        
        [HideInCallstack] public static void Trace<TCaller>(string message) => Trace(typeof(TCaller), message);
        [HideInCallstack] public static void Debug<TCaller>(string message) => Debug(typeof(TCaller), message);
        [HideInCallstack] public static void Information<TCaller>(string message) => Information(typeof(TCaller), message);
        [HideInCallstack] public static void Warning<TCaller>(string message) => Warning(typeof(TCaller), message);
        [HideInCallstack] public static void Error<TCaller>(string message) => Error(typeof(TCaller), message);
        [HideInCallstack] public static void Critical<TCaller>(string message) => Critical(typeof(TCaller), message);
        
        // 型推論で<TCaller>を指定しなくても済むようにするインターフェース
        [HideInCallstack] public static void Trace<TCaller>(TCaller _, string message) => Trace<TCaller>(message);
        [HideInCallstack] public static void Debug<TCaller>(TCaller _, string message) => Debug<TCaller>(message);
        [HideInCallstack] public static void Information<TCaller>(TCaller _, string message) => Information<TCaller>(message);
        [HideInCallstack] public static void Warning<TCaller>(TCaller _, string message) => Warning<TCaller>(message);
        [HideInCallstack] public static void Error<TCaller>(TCaller _, string message) => Error<TCaller>(message);
        [HideInCallstack] public static void Critical<TCaller>(TCaller _, string message) => Critical<TCaller>(message);
        
        
        #region Register / Unregister Sinks

        public static void RegisterSink(ISink sink, SinkFilterConfig filterConfig)
        {
            LogDispatcher?.Register(sink, filterConfig.categoryLogLevels);
        }
        
        public static void RegisterSink(ISink sink, IEnumerable<CategoryMinimumLogLevel> categoryLogLevels)
        {
            LogDispatcher?.Register(sink, categoryLogLevels);
        }
        
        public static void RegisterSink(ISink sink, string category, LogLevel logLevel)
        {
            LogDispatcher?.Register(sink, category, logLevel);
        }
        
        public static void UnregisterSink(ISink sink)
        {
            LogDispatcher?.Unregister(sink);
        }
        
        #endregion
    }
}