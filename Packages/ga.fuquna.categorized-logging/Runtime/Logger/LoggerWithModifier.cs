namespace CategorizedLogging
{
    public class LoggerWithModifier : ILogger
    {
        private readonly ILogger _logger;
        private readonly ILogModifier _modifier;
        
        
        public LoggerWithModifier(ILogger logger, ILogModifier modifier)
        {
            _logger = logger;
            _modifier = modifier;
        }
        
        public LogEntry CreateLogEntry(LogLevel logLevel, string message)
        {
            var originalLogEntry = _logger.CreateLogEntry(logLevel, message);
            return _modifier.Modify(originalLogEntry);
        }
    }
}