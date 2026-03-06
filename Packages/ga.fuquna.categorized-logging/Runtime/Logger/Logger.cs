using System.Collections.Generic;

namespace CategorizedLogging
{
    /// <summary>
    /// カテゴリを紐づけたロガーのインターフェース
    /// </summary>
    public class Logger : ILogger
    {
        private static readonly Dictionary<string, Logger> Loggers = new();
        
        
        public static Logger Get<T>(T _) => Get<T>();
        public static Logger Get<T>() => Get(typeof(T).Name);
        public static Logger Get(string category)
        {
            if (!Loggers.TryGetValue(category, out var logger))
            {
                logger = new Logger(category);
                Loggers[category] = logger;
            }
            return logger;
        }
        
        
        private readonly string _category;
        
        public Logger(string category)
        {
            _category = category;
        }

        public void EmitLog(LogLevel logLevel, string message) => Log.EmitLog(_category, logLevel, message);
    }
}