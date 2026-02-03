using System;
using System.Collections.Generic;

namespace CategorizedLogging
{
    /// <summary>
    /// LogLevel per category settings for logger subscription
    /// </summary>
    [Serializable]
    public class LoggerSetting
    {
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