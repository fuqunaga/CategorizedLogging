using System;
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
        public static ILogger Logger { get; set; } = new LogDispatcher();


        public static void EmitLog(in LogEntry logEntry) => Logger?.Log(logEntry);
        public static void EmitLog(string category, LogLevel logLevel, string message) => EmitLog(new LogEntry(logLevel, category, message));
        public static void EmitLog(Type typeForCategory, LogLevel logLevel, string message) => EmitLog(typeForCategory.Name, logLevel, message);
        public static void EmitLog<TCaller>(LogLevel logLevel, string message) => EmitLog(typeof(TCaller), logLevel, message);
        public static void EmitLog<TCaller>(TCaller _, LogLevel logLevel, string message) => EmitLog<TCaller>(logLevel, message);
        
        
        public static void Trace(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Trace, message);
        public static void Debug(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Debug, message);
        public static void Information(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Information, message);
        public static void Warning(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Warning, message);
        public static void Error(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Error, message);
        public static void Critical(Type typeForCategory, string message)　=> EmitLog(typeForCategory, LogLevel.Critical, message);
        
        public static void Trace<TCaller>(string message) => Trace(typeof(TCaller), message);
        public static void Debug<TCaller>(string message) => Debug(typeof(TCaller), message);
        public static void Information<TCaller>(string message) => Information(typeof(TCaller), message);
        public static void Warning<TCaller>(string message) => Warning(typeof(TCaller), message);
        public static void Error<TCaller>(string message) => Error(typeof(TCaller), message);
        public static void Critical<TCaller>(string message) => Critical(typeof(TCaller), message);
        
        // 型推論で<TCaller>を指定しなくても済むようにするインターフェース
        public static void Trace<TCaller>(TCaller _, string message) => Trace<TCaller>(message);
        public static void Debug<TCaller>(TCaller _, string message) => Debug<TCaller>(message);
        public static void Information<TCaller>(TCaller _, string message) => Information<TCaller>(message);
        public static void Warning<TCaller>(TCaller _, string message) => Warning<TCaller>(message);
        public static void Error<TCaller>(TCaller _, string message) => Error<TCaller>(message);
        public static void Critical<TCaller>(TCaller _, string message) => Critical<TCaller>(message);
    }
}