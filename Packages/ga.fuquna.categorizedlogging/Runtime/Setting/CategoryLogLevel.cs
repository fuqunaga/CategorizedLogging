using System;
using UnityEngine;

namespace CategorizedLogging
{
    [Serializable]
    public struct CategoryMinimumLogLevel
    {
        public string category;
        [Tooltip("Minimum log level to log for this category")]
        public LogLevel logLevel;
    }
}