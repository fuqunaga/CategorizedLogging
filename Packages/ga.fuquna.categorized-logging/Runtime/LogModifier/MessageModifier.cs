using System;
using UnityEngine;

namespace CategorizedLogging
{
    public class MessageModifier : ILogModifier
    {
        private Func<string, string> ModifyMessageFunc { get; }

        
        public MessageModifier(Func<string, string> modifyMessageFunc)
        {
            ModifyMessageFunc = modifyMessageFunc;
        }
        
        [HideInCallstack]
        public LogEntry Modify(in LogEntry logEntry)
        {
            var modifiedMessage = ModifyMessageFunc?.Invoke(logEntry.Message) ?? logEntry.Message;
            return new LogEntry(logEntry.LogLevel, logEntry.Category, modifiedMessage);
        }

        
        protected bool Equals(MessageModifier other)
        {
            return Equals(ModifyMessageFunc, other.ModifyMessageFunc);
        }

        public bool Equals(ILogModifier other) => other is MessageModifier otherMessageModifier && Equals(otherMessageModifier);
        public override bool Equals(object obj) => obj is MessageModifier other && Equals(other);
        

        public override int GetHashCode()
        {
            return (ModifyMessageFunc != null ? ModifyMessageFunc.GetHashCode() : 0);
        }
    }
    
    
    public static class LoggerExtensionsMessageModifier
    {
        public static ILogger ModifyMessage(this ILogger logger, Func<string, string> modifyMessageFunc)
        {
            return logger.AddModifier(new MessageModifier(modifyMessageFunc));
        }
    }
}