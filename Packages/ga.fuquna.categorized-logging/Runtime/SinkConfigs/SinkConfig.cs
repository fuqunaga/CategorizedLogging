using System;
using System.Collections.Generic;
using System.Linq;

namespace CategorizedLogging
{
    /// <summary>
    /// LogLevel per category settings for logger subscription
    /// </summary>
    [Serializable]
    public class SinkFilterConfig
    {
        public static SinkFilterConfig Create(LogLevel level)
        {
            return new SinkFilterConfig
            {
                categoryLogLevels = new List<CategoryMinimumLogLevel>()
                {
                    new()
                    {
                        category = "*",
                        logLevel = level
                    }
                }
            };
        }
        
        public static SinkFilterConfig Create(LogLevel level, params string[] categories) 
        {
            return Create(level, categories.AsEnumerable());
        }
        
        public static SinkFilterConfig Create(LogLevel level, IEnumerable<string> categories)
        {
            return new SinkFilterConfig
            {
                categoryLogLevels = categories.Select(category => new CategoryMinimumLogLevel()
                {
                    category = category,
                    logLevel = level
                }).ToList()
            };
        }
        
        
        public List<CategoryMinimumLogLevel> categoryLogLevels = new()
        {
            new CategoryMinimumLogLevel()
            {
                category = "*",
                logLevel = LogLevel.Information
            }
        };
    }
}