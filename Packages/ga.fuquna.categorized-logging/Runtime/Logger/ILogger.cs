namespace CategorizedLogging
{
    /// <summary>
    /// 呼び出し側でカテゴリを指定せずにログを出力するためのインターフェース
    /// </summary>
    public interface ILogger
    {
        void EmitLog(LogLevel logLevel, string message);
    }
    
    public static class LoggerExtensions
    {
        public static void Trace<TLogger>(this TLogger logger, string message) where TLogger : ILogger 
            => logger.EmitLog(LogLevel.Trace, message);

        public static void Debug<TLogger>(this TLogger logger, string message) where TLogger : ILogger 
            => logger.EmitLog(LogLevel.Debug, message);

        public static void Information<TLogger>(this TLogger logger, string message) where TLogger : ILogger 
            => logger.EmitLog(LogLevel.Information, message);

        public static void Warning<TLogger>(this TLogger logger, string message) where TLogger : ILogger 
            => logger.EmitLog(LogLevel.Warning, message);

        public static void Error<TLogger>(this TLogger logger, string message) where TLogger : ILogger 
            => logger.EmitLog(LogLevel.Error, message);

        public static void Critical<TLogger>(this TLogger logger, string message) where TLogger : ILogger 
            => logger.EmitLog(LogLevel.Critical, message);
    }
}