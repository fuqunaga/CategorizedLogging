using System;

namespace CategorizedLogging
{
    public readonly struct LogEntry
    {
        public DateTime Timestamp { get; }
        public LogLevel LogLevel { get; }
        public string Category { get; }
        public string Message { get; }

        public LogEntry(LogLevel logLevel, string category, string message)
        {
            Timestamp = DateTime.Now;
            LogLevel = logLevel;
            Category = category;
            Message = message;
        }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}][{LogLevel}][{Category}] {Message}";
        }
    }
}