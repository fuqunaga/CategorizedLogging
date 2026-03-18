using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScorpionLog
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
        None
    }

    public static class LogLevelExtensions
    {
        public static Dictionary<LogLevel, string> LogLevelStringColor { get; } = new()
        {
            { LogLevel.Trace, ColorUtility.ToHtmlStringRGB(new Color(0.5f, 0.5f, 0.5f)) },
            { LogLevel.Debug, ColorUtility.ToHtmlStringRGB(new Color(0.4f, 0.7f, 0.9f)) },
            { LogLevel.Information, ColorUtility.ToHtmlStringRGB(new Color(0.95f, 0.95f, 0.95f)) },
            { LogLevel.Warning, ColorUtility.ToHtmlStringRGB(new Color(1f, 0.8f, 0f)) },
            { LogLevel.Error, ColorUtility.ToHtmlStringRGB(new Color(1f, 0.3f, 0.2f)) },
            { LogLevel.Fatal, ColorUtility.ToHtmlStringRGB(new Color(0.9f, 0.2f, 0.9f)) },
            { LogLevel.None, ColorUtility.ToHtmlStringRGB(Color.clear) }
        };
        
        public static string ToShortString(this LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "ERROR",
                LogLevel.Fatal => "FATAL",
                LogLevel.None => "NONE",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            };
        }

        public static string ToShortStringWithColor(this LogLevel logLevel)
        {
            var color = LogLevelStringColor[logLevel];
            var shortString = logLevel.ToShortString();
            return $"<color=#{color}>{shortString}</color>";
        }
    }
}