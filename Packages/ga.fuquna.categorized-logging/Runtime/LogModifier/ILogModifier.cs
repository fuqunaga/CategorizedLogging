using System;
using UnityEngine;

namespace CategorizedLogging
{
    public interface ILogModifier : IEquatable<ILogModifier>
    {
        LogEntry Modify(in LogEntry logEntry);
    }
    
    
    public static class LoggerExtensionsForLogModifier
    {
        public static ILogger AddModifier(this ILogger logger, ILogModifier modifier)
        {
            return new LoggerWithModifier(logger, modifier);
        }
    }
}