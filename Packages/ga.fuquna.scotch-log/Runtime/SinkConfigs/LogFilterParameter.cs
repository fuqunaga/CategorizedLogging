using System;
using System.Collections.Generic;
using System.Linq;
using ScotchLog.Scope;
using UnityEngine.Pool;

namespace ScotchLog
{
    [Serializable]
    public class LogFilterParameter : IEquatable<LogFilterParameter>
    {
        public string scopeName;
        public LogLevel minimumLogLevel;
        public List<LogScopeProperty> properties;


        public LogFilterParameter()
        {
        }

        public LogFilterParameter(LogFilterParameter other)
        {
            scopeName = other.scopeName;
            if (other.properties != null)
            {
                properties = new List<LogScopeProperty>(other.properties.Distinct());
            }
            minimumLogLevel = other.minimumLogLevel;
        }

        
        public bool IsMatch(LogEntry logEntry)
        {
            if (logEntry.LogLevel < minimumLogLevel)
            {
                return false;
            }
            
            var scope = logEntry.Span;
            while (scope != null)
            {
                if (IsMatch(scope))
                {
                    return true;
                }
                
                scope = scope.Parent;
            }

            return false;
        }
        
        public bool IsMatch(LogSpanRecord spanRecord)
        {
            return IsMatchName(spanRecord)
                   && IsMatchProperties(spanRecord);
        }

        public bool IsMatchName(LogSpanRecord spanRecord)
        {
            return (scopeName == "*") 
                    ||(scopeName == spanRecord.Name);
        }

        public bool IsMatchProperties(LogSpanRecord spanRecord)
        {
            if (properties == null || properties.Count == 0)
            {
                return true;
            }

            if (spanRecord.Properties == null)
            {
                return false;
            }

            foreach (var (key, value) in properties)
            {
                if (!spanRecord.Properties.TryGetValue(key, out var recordValue) || recordValue != value)
                {
                    return false;
                }
            }

            return true;
        }


        #region Equality members

        public bool Equals(LogFilterParameter other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (scopeName != other.scopeName || minimumLogLevel != other.minimumLogLevel) return false;

            // propertiesの順序不問・値比較（重複考慮）
            if (properties == null && other.properties == null) return true;
            if (properties == null || other.properties == null) return false;
            if (properties.Count != other.properties.Count) return false;

            using var _ = ListPool<LogScopeProperty>.Get(out var otherCopy);
            otherCopy.AddRange(other.properties);
            return properties.All(item => otherCopy.Remove(item));
        }

        public override bool Equals(object obj)
        {
            return obj is LogFilterParameter other && Equals(other);
        }

        public override int GetHashCode()
        {
            var propertiesHash = 0;
            if (properties != null)
            {
                propertiesHash = properties.Aggregate(propertiesHash, (current, item) => current ^ item.GetHashCode());
            }
            return HashCode.Combine(scopeName, propertiesHash, (int)minimumLogLevel);
        }

        #endregion
    }
}