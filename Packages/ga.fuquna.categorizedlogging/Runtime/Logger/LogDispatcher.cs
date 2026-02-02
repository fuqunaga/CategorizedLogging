using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CategorizedLogging
{
    public class LogDispatcher : ILogger
    {
        private readonly Dictionary<LogLevel, HashSet<ILogger>> _anyCategoryLoggers = new();
        private readonly Dictionary<string, Dictionary<LogLevel, HashSet<ILogger>>> _specificLoggers = new();
        
        private readonly Dictionary<string, Dictionary<LogLevel, HashSet<ILogger>>> _cachedLoggerTable = new();
        private readonly object _lockObject = new();
        
        private bool _needsCacheRefresh = false;
        
        
        public void Log(in LogEntry logEntry)
        {
            var category = logEntry.Category;
            var logLevel = logEntry.LogLevel;
            
            if (logLevel == LogLevel.None)
            {
                return;
            }
            
            if (_needsCacheRefresh)
            {
                _cachedLoggerTable.Clear();
                _needsCacheRefresh = false;
            }
            
            if ( !_cachedLoggerTable.TryGetValue(category, out var logLevelTable))
            {
                logLevelTable = new Dictionary<LogLevel, HashSet<ILogger>>();
                _cachedLoggerTable[category] = logLevelTable;
            }
            
            if ( !logLevelTable.TryGetValue(logLevel, out var hashSet))
            {
                hashSet = CreateLoggerCache(category, logLevel);
                logLevelTable[logLevel] = hashSet;
            }
            
            foreach(var logger in hashSet)
            {
                logger.Log(logEntry);
            }
        }


        public void Subscribe(ILogger logger, IEnumerable<CategoryMinimumLogLevel> categoryLogLevels)
        {
            foreach (var minimumLogLevel in categoryLogLevels)
            {
                for(var level = minimumLogLevel.logLevel; level <= LogLevel.Critical; level++)
                {
                    Subscribe(logger, minimumLogLevel.category, level);
                }
            }
        }
        
        
        [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
        public void Subscribe(ILogger logger, string category, LogLevel logLevel)
        {
            var anyCategory = string.IsNullOrEmpty(category) ||  category == "*";

            var changed = false;

            Dictionary<LogLevel, HashSet<ILogger>> logLevelTable = null;
            
            if (anyCategory)
            {
                logLevelTable = _anyCategoryLoggers;
            }
            else
            {
                if (!_specificLoggers.TryGetValue(category, out logLevelTable))
                {
                    logLevelTable = new Dictionary<LogLevel, HashSet<ILogger>>();
                    lock (_lockObject)
                    {
                        _specificLoggers[category] = logLevelTable;
                    }
                }
            }
            
            changed = SetLoggerToDictionary(logLevelTable, logLevel, logger);
            
            _needsCacheRefresh = _needsCacheRefresh || changed;
        }
        
        
        public void Unsubscribe(ILogger logger)
        {
            lock (_lockObject)
            {
                foreach (var logLevelTable in _anyCategoryLoggers.Values)
                {
                    logLevelTable.Remove(logger);
                }

                foreach (var logLevelTable in _specificLoggers.Values.SelectMany(categoryTable => categoryTable.Values))
                {
                    logLevelTable.Remove(logger);
                }
                
                _needsCacheRefresh = true;
            }
        }
        
        
        private bool SetLoggerToDictionary<TKey>(Dictionary<TKey, HashSet<ILogger>> dictionary, TKey key, ILogger logger)
        {
            lock (_lockObject)
            {
                if (!dictionary.TryGetValue(key, out var loggerSet))
                {
                    loggerSet = new HashSet<ILogger>();
                    dictionary[key] = loggerSet;
                }

                return loggerSet.Add(logger);
            }
        }

        private HashSet<ILogger> CreateLoggerCache(string category, LogLevel logLevel)
        {
            lock (_lockObject)
            {
                var result = new HashSet<ILogger>(_anyCategoryLoggers.GetValueOrDefault(logLevel));
                
                if (_specificLoggers.TryGetValue(category, out var logLabelTable) 
                    && logLabelTable.TryGetValue(logLevel, out var hashSet))
                {
                    result.UnionWith(hashSet);
                }

                return result;
            }
        }
    }
}