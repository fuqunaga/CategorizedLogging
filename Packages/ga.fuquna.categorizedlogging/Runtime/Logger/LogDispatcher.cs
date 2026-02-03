using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CategorizedLogging
{
    public class LogDispatcher : ILogger
    {
        public static bool IsAnyCategory(string category) => category == "*";
        
        
        private readonly Dictionary<LogLevel, HashSet<ILogger>> _anyCategoryLoggers = new();
        private readonly Dictionary<string, Dictionary<LogLevel, HashSet<ILogger>>> _specificLoggers = new();
        private readonly Dictionary<string, Dictionary<LogLevel, HashSet<ILogger>>> _cachedLoggerTable = new();
        private readonly object _lockObject = new();
        private bool _needsCacheRefresh = false;


#if UNITY_EDITOR
        public LogDispatcher()
        {
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    lock (_lockObject)
                    {
                        _anyCategoryLoggers.Clear();
                        _specificLoggers.Clear();
                        _cachedLoggerTable.Clear();
                        _needsCacheRefresh = false;
                    }
                }
            };
        }
#endif
        
        
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


        /// <summary>
        /// ILoggerを登録する
        /// categoryに"*"を指定すると全カテゴリに登録される
        /// ただし"*"以外のカテゴリに対して個別に登録されたログレベルのほうが優先される
        /// </summary>
        public void Register(ILogger logger, IEnumerable<CategoryMinimumLogLevel> categoryLogLevels)
        {
            Unregister(logger);
            
            foreach (var categoryLogLevel in categoryLogLevels)
            {
                for(var level = categoryLogLevel.logLevel; level <= LogLevel.Critical; level++)
                {
                    Register(logger, categoryLogLevel.category, level);
                }
            }
        }
        
        
        [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
        public void Register(ILogger logger, string category, LogLevel logLevel)
        {
            var changed = false;

            Dictionary<LogLevel, HashSet<ILogger>> logLevelTable = null;
            
            if (IsAnyCategory(category))
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
            
            _needsCacheRefresh |= changed;
        }
        
        
        public void Unregister(ILogger logger)
        {
            var changed = false;

            lock (_lockObject)
            {
                foreach (var logLevelTable in _anyCategoryLoggers.Values)
                {
                    changed |= logLevelTable.Remove(logger);
                }

                foreach (var logLevelTable in _specificLoggers.Values.SelectMany(categoryTable => categoryTable.Values))
                {
                    changed |= logLevelTable.Remove(logger);
                }
            }
            
            _needsCacheRefresh |= changed;
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

        /// <summary>
        /// CategoryとLogLevelに基づいて対象のILoggerのキャッシュを作成する
        ///
        /// AnyCategoryで指定されていてもspecificLoggersで指定されているILoggerはspecificLoggersを優先する
        /// </summary>
        private HashSet<ILogger> CreateLoggerCache(string category, LogLevel logLevel)
        {
            lock (_lockObject)
            {
                var categoryLoggers = _specificLoggers.GetValueOrDefault(category);
                var specificCategoryLoggers = categoryLoggers?
                                                  .SelectMany(table => table.Value)
                                                  .Distinct()
                                              ?? Enumerable.Empty<ILogger>();
                
                var result = new HashSet<ILogger>(
                    _anyCategoryLoggers.GetValueOrDefault(logLevel)
                    ?? Enumerable.Empty<ILogger>()
                );
                result.ExceptWith(specificCategoryLoggers);
                
                if (categoryLoggers?.TryGetValue(logLevel, out var hashSet) ?? false)
                {
                    result.UnionWith(hashSet);
                }

                return result;
            }
        }
    }
}