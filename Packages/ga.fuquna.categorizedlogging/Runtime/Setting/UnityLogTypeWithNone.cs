using UnityEngine;

namespace CategorizedLogging
{
    public enum UnityLogTypeWithNone
    {
        Error,
        Assert,
        Warning,
        Log,
        Exception,
        None
    }
    
    
    public static class UnityLogTypeWithNoneExtensions
    {
        public static LogType? ToLogType(this UnityLogTypeWithNone logTypeWithNone)
        {
            return logTypeWithNone switch
            {
                UnityLogTypeWithNone.Error => LogType.Error,
                UnityLogTypeWithNone.Assert => LogType.Assert,
                UnityLogTypeWithNone.Warning => LogType.Warning,
                UnityLogTypeWithNone.Log => LogType.Log,
                UnityLogTypeWithNone.Exception => LogType.Exception,
                UnityLogTypeWithNone.None => null,
                _ => null
            };
        }
    }
}