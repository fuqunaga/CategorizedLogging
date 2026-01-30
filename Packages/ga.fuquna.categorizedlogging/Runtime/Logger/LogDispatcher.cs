using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CategorizedLogging
{
    public class LogDispatcher : ILogger
    {
        private readonly HashSet<ILogger> _anyLoggers = new();
        private readonly Dictionary<string, HashSet<ILogger>> _specificCategoryLoggers = new();
        private readonly Dictionary<LogLevel, HashSet<ILogger>> _specificLogLevelLoggers = new();
        private readonly Dictionary<(string, LogLevel), HashSet<ILogger>> _specificLoggers = new();
        
        private readonly Dictionary<(string, LogLevel), HashSet<ILogger>> _cachedLoggerTable = new();
        private readonly object _lockObject = new();
        
        
        public void Log(in LogEntry logEntry)
        {
            var category = logEntry.Category;
            var logLevel = logEntry.LogLevel;
            
            if (logLevel == LogLevel.None)
            {
                return;
            }
            
            if ( !_cachedLoggerTable.TryGetValue((category, logLevel), out var list))
            {
                list = CreateLoggerCache(category, logLevel);
                _cachedLoggerTable[(category, logLevel)] = list;
            }
            
            foreach(var logger in list)
            {
                logger.Log(logEntry);
            }
        }
        
        
        [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
        public void Subscribe(ILogger logger, string category = "*", LogLevel? logLevel = null)
        {
            var anyCategory = string.IsNullOrEmpty(category) ||  category == "*";
            var anyLevel = logLevel == null;

            lock (_lockObject)
            {
                if (anyCategory && anyLevel)
                {
                    _anyLoggers.Add(logger);
                    _cachedLoggerTable.Clear();
                }
                else if (anyCategory)
                {
                    SetLoggerToDictionary(_specificLogLevelLoggers, logLevel.Value, logger);
                }
                else if (anyLevel)
                {
                    SetLoggerToDictionary(_specificCategoryLoggers, category, logger);
                }
                else
                {
                    SetLoggerToDictionary(_specificLoggers, (category, logLevel.Value), logger);
                }
            }
        }
        
        
        private void SetLoggerToDictionary<TKey>(Dictionary<TKey, HashSet<ILogger>> dictionary, TKey key, ILogger logger)
        {
            if (!dictionary.TryGetValue(key, out var loggerSet))
            {
                loggerSet = new HashSet<ILogger>();
                dictionary[key] = loggerSet;
            }
            loggerSet.Add(logger);
            _cachedLoggerTable.Clear();
        }

        private HashSet<ILogger> CreateLoggerCache(string category, LogLevel logLevel)
        {
            lock (_lockObject)
            {
                var result = new HashSet<ILogger>(_anyLoggers);

                if (_specificCategoryLoggers.TryGetValue(category, out var anyLevelLoggers))
                {
                    result.UnionWith(anyLevelLoggers);
                }

                if (_specificLogLevelLoggers.TryGetValue(logLevel, out var anyCategoryLoggers))
                {
                    result.UnionWith(anyCategoryLoggers);
                }

                if (_specificLoggers.TryGetValue((category, logLevel), out var specificLoggers))
                {
                    result.UnionWith(specificLoggers);
                }

                return result;
            }
        }
    }
}